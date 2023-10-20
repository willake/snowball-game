using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Game.Saves;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.WebRequests
{
    public class WebRequestManager
    {
        public async UniTask<WebRequestResult> UploadPlayData(GameStatisticsDataV1 data)
        {
            string dataToSend = JsonConvert.SerializeObject(data);
            Debug.Log("JSON Data: " + data);
            UnityWebRequest request = new UnityWebRequest(
                Consts.DATA_COLLECTION_URL, UnityWebRequest.kHttpVerbPOST);
            byte[] bytes = Encoding.UTF8.GetBytes(dataToSend);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("content-Type", "application/json");

            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    Debug.Log("Form upload complete!");
                }

                return new WebRequestResult
                {
                    isSuccess = request.result == UnityWebRequest.Result.Success,
                    responseCode = request.responseCode,
                    message = request.error
                };
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return new WebRequestResult
                {
                    isSuccess = false,
                    responseCode = request.responseCode,
                    message = request.error
                };
            }
        }

        public struct WebRequestResult
        {
            public bool isSuccess;
            public long responseCode;
            public string message;
        }
    }
}