using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using UnityEngine.SceneManagement;
[System.Serializable]
[DynamoDBTable("User_Data")]
public class TestUserData
{
    [DynamoDBHashKey] // Hash key.
    public string id { get; set; }
    [DynamoDBProperty]
    public string password { get; set; }
    [DynamoDBProperty]
    public string guid { get; set; }

}
[System.Serializable]
[DynamoDBTable("User_Item_Data")]
public class TestUserItemData
{
    [DynamoDBHashKey] // Hash key.
    public string Guid { get; set; }
    [DynamoDBProperty]
    public string[] Item_Data { get; set; }
}
[System.Serializable]
public class UserDatabase1
{
    public UserDatabase1
    (string _ID, string _Password, string _Guid)
    { ID = _ID; Password = _Password; Guid = _Guid; }
    public string ID, Password, Guid;
}
[System.Serializable]
public class UserItemDatabase
{
    public UserItemDatabase
    (string _Guid, string[] _Item_Data)
    { Guid = _Guid; Item_Data = _Item_Data; }
    public string Guid;
    public string[] Item_Data;
}

public class TestDataHandler : MonoBehaviour
{
    public DynamoDBContext context;
    public AmazonDynamoDBClient DBclient;
    public CognitoAWSCredentials credentials;
    public List<UserDatabase1> UserDatabaseList;
    public List<UserItemDatabase> UserItemDatabaseList;
    [Header("로그인정보")]
    [SerializeField] InputField InputID;
    [SerializeField] InputField InputPW;
    private void Awake()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        credentials = new CognitoAWSCredentials("ap-northeast-2:f42e0252-00be-4237-bf59-e359d6be9438", RegionEndpoint.APNortheast2);
        DBclient = new AmazonDynamoDBClient(credentials, RegionEndpoint.APNortheast2);
        context = new DynamoDBContext(DBclient);
    }
    private void Start()
    {
    }
    public void TestUserCall()
    {
        TestUserData searchResult;
        context.LoadAsync<TestUserData>(InputID.text, (result) =>
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
            UserDatabaseList.Add(userdata);
            TestUserItemData();
        }, null);
    }
    public void TestUserItemData()
    {
        TestUserItemData searchResult;
        context.LoadAsync<TestUserItemData>(UserDatabaseList[0].Guid, (AmazonDynamoDBResult<TestUserItemData> result) =>
        {
            // id가 InputID.text인 캐릭터 정보를 DB에서 받아옴
            if (result.Exception != null)
            {
                Debug.LogException(result.Exception);
                return;
            }
            UserItemDatabase userdata = new UserItemDatabase(
               result.Result.Guid,
               result.Result.Item_Data
           );
            searchResult = result.Result;
            UserItemDatabaseList.Add(userdata);
        }, null);
    }
}
