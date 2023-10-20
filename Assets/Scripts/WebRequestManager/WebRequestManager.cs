using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Saves;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.WebRequests
{
    public class WebRequestManager : MonoBehaviour
    {
        public async UniTask<Tuple<bool, string>> UploadPlayData(GameStatisticsDataV1 data)
        {
            string dataToSend = JsonConvert.SerializeObject(data);
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection(dataToSend));
            UnityWebRequest www = UnityWebRequest.Post(Consts.DATA_COLLECTION_URL, formData);
            www.SetRequestHeader("Content-Type", "application/json");
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return new Tuple<bool, string>(false, www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                return new Tuple<bool, string>(true, www.error);
            }
        }
    }
}