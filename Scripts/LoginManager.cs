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
    [Header("�α���ȭ��")]
    [SerializeField] GameObject SignInBackBtn;
    [SerializeField] GameObject SignInBtn;
    [SerializeField] GameObject m_SignInUi;

    TestDataHandler tdh;

    [Header("ȸ�������г�")]
    [SerializeField] InputField InputIDSignUp;
    [SerializeField] InputField InputPWSignUp;

    [Header("�α�������")]
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
    public void CreateUser() //ĳ���� ������ DB�� �ø���
    {
        context.LoadAsync<UserData>(InputIDSignUp.text, (AmazonDynamoDBResult<UserData> result) =>
        {
            if (result.Result.id == InputIDSignUp.text)
            {
                Debug.Log("�̹� �����ϴ� ���̵��Դϴ�.");
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
            //id�� happy, password�� ���θ����ȣ, UserIndex�� 0 ĳ���� ������ DB�� ����
            if (result.Exception == null)
            {
                Debug.Log(newData.guid);
                Debug.Log("Success!");
            }
            else
                Debug.Log(result.Exception);
        });
    }
    public void FindUser() //DB���� ĳ���� ���� �ޱ�
    {
        UserData searchResult;
        AmazonDynamoDBResult<UserData> result = null;
        context.LoadAsync<UserData>(InputID.text, (result) =>
        {
            // id�� InputID.text�� ĳ���� ������ DB���� �޾ƿ�
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                return;
            }
            if (result.Result.id != InputID.text)
            {
                Debug.Log("�ùٸ� ���̵𰡾ƴմϴ�");
                return;
            }
            else if (result.Result.password != InputPW.text)
            {
                Debug.Log("�ùٸ� ��й�ȣ���ƴմϴ�");
                return;
            }
            else if (result.Result.id == InputID.text)
            {
                if (result.Result.password == InputPW.text)
                {
                    Debug.Log("�α��� ����");
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
    //        Debug.Log("id�� �Է��ϼ���");
    //        return;
    //    }
    //    else if (id != null && pw == null)
    //    {
    //        Debug.Log("pw�� �Է��ϼ���");
    //        return;
    //    }
    //    else if (ldh.UserDatabaseList.Find(x => x.ID == id) == null)
    //    {
    //        Debug.Log("�ùٸ��� ���� ���̵��Դϴ�.");
    //        return;
    //    }
    //    else if (ldh.UserDatabaseList.Find(x => x.ID == id) != null)
    //    {
    //        UserDatabase inputUserdata = ldh.UserDatabaseList.Find(x => x.ID == id);
    //        if (inputUserdata.Password != pw)
    //        {
    //            Debug.Log("�ùٸ������� ��й�ȣ�Դϴ�");
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
