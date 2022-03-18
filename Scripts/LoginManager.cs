using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
[System.Serializable]
[DynamoDBTable("User_Data")]
public class UserData
{
    [DynamoDBHashKey] // Hash key.
    public string id { get; set; }
    [DynamoDBProperty]
    public string password { get; set; }
    [DynamoDBProperty]
    public string guid { get; set; }

}

public class LoginManager : MonoBehaviour
{
    [Header("로그인화면")]
    [SerializeField] GameObject SignInBackBtn;
    [SerializeField] GameObject SignInBtn;
    [SerializeField] GameObject m_SignInUi;

    TestDataHandler tdh;

    [Header("회원가입패널")]
    [SerializeField] InputField InputIDSignUp;
    [SerializeField] InputField InputPWSignUp;

    [Header("로그인정보")]
    [SerializeField] InputField InputID;
    [SerializeField] InputField InputPW;
    DynamoDBContext context;
    AmazonDynamoDBClient DBclient;
    CognitoAWSCredentials credentials;
    private void Awake()
    {
        credentials = new CognitoAWSCredentials("ap-northeast-2:f42e0252-00be-4237-bf59-e359d6be9438", RegionEndpoint.APNortheast2);
        DBclient = new AmazonDynamoDBClient(credentials, RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(DBclient);
    }
    public void Start()
    {
        tdh = GameObject.Find("LoginDataHandler").GetComponent<TestDataHandler>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InputPW.Select();
        }
    }
    public void CreateUser() //캐릭터 정보를 DB에 올리기
    {
        context.LoadAsync<UserData>(InputIDSignUp.text, (AmazonDynamoDBResult<UserData> result) =>
        {
            if (result.Result.id == InputIDSignUp.text)
            {
                Debug.Log("이미 존재하는 아이디입니다.");
                return;
            }
        }, null);
        UserData newData = new UserData
        {
            id = InputIDSignUp.text,
            password = InputPWSignUp.text,
            guid = Guid.NewGuid().ToString()
        };
        context.SaveAsync(newData, (result) =>
        {
            //id가 happy, password가 새로만든번호, UserIndex가 0 캐릭터 정보를 DB에 저장
            if (result.Exception == null)
            {
                Debug.Log(newData.guid);
                Debug.Log("Success!");
            }
            else
                Debug.Log(result.Exception);
        });
    }
    public void FindUser() //DB에서 캐릭터 정보 받기
    {
        UserData searchResult;
        AmazonDynamoDBResult<UserData> result = null;
        context.LoadAsync<UserData>(InputID.text, (result) =>
        {
            // id가 InputID.text인 캐릭터 정보를 DB에서 받아옴
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                return;
            }
            if (result.Result.id != InputID.text)
            {
                Debug.Log("올바른 아이디가아닙니다");
                return;
            }
            else if (result.Result.password != InputPW.text)
            {
                Debug.Log("올바른 비밀번호가아닙니다");
                return;
            }
            else if (result.Result.id == InputID.text)
            {
                if (result.Result.password == InputPW.text)
                {
                    Debug.Log("로그인 성공");
                    SceneManager.LoadScene("MainMenu");
                }
            }
            UserDatabase1 userdata = new UserDatabase1(
                result.Result.id,
                result.Result.password,
                result.Result.guid
            );
            searchResult = result.Result;
            Debug.Log(userdata.Guid);
            tdh.UserDatabaseList.Add(userdata);
        }, null);
    }

    public void SignUpButton()
    {
        iTween.PunchScale(SignInBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            iTween.MoveTo(m_SignInUi, iTween.Hash("x", 0, "time", 1, "islocal", true));
        }));
    }
    public void SignUpBackButton()
    {
        iTween.PunchScale(SignInBackBtn, iTween.Hash("x", 0.9f, "y", 0.9f, "time", 0.3f));
        StartCoroutine(WaitAndCall(0.3f, () =>
        {
            iTween.MoveTo(m_SignInUi, iTween.Hash("x", 2000, "time", 1, "islocal", true));
        }));
    }
    IEnumerator WaitAndCall(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }


    //public void UserDataAccess()
    //{
    //    string id;
    //    string pw;

    //    id = InputID.text;
    //    pw = InputPW.text;
    //    if (id == null)
    //    {
    //        Debug.Log("id를 입력하세요");
    //        return;
    //    }
    //    else if (id != null && pw == null)
    //    {
    //        Debug.Log("pw를 입력하세요");
    //        return;
    //    }
    //    else if (ldh.UserDatabaseList.Find(x => x.ID == id) == null)
    //    {
    //        Debug.Log("올바르지 않은 아이디입니다.");
    //        return;
    //    }
    //    else if (ldh.UserDatabaseList.Find(x => x.ID == id) != null)
    //    {
    //        UserDatabase inputUserdata = ldh.UserDatabaseList.Find(x => x.ID == id);
    //        if (inputUserdata.Password != pw)
    //        {
    //            Debug.Log("올바르지않은 비밀번호입니다");
    //            return;
    //        }
    //        else if (inputUserdata.Password == pw)
    //        {
    //            SceneManager.LoadScene(1);
    //        }
    //    }
    //}
    //public void CreateAccount()
    //{

    //    UserDatabase userdata = new UserDatabase(InputIDSignUp.text, InputPWSignUp.text, (ldh.UserDatabaseList.Count).ToString());
    //    ldh.UserDatabaseList.Add(userdata);

    //    ldh.UserSave();
    //}
}
