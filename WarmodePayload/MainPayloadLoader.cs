using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using UnityEngine;

namespace WarmodePayload
{
    public static class MainPayloadLoader
    {
        public static GameObject MainPayloadLoaderGameObject;

        [DllExport("OnMainPayloadLoad", CallingConvention.Cdecl)]
        public static void OnMainPayloadLoad()
        {
            MainPayloadLoaderGameObject = new GameObject();
            MainPayloadLoaderGameObject.AddComponent<MainPayloadFun>();
            Object.DontDestroyOnLoad(MainPayloadLoaderGameObject);
        }
    }
}