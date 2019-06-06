using System;
using System.Collections.Generic;
using System.Text;

namespace ExactOnline.CustomAuth
{
    public static class Globals
    {
        // TOKENS
        public static readonly String EXACT_TOKEN = "Exact_Token";
        public static readonly String EXACT_REFRESH_TOKEN = "Exact_Refresh_Token";
        public static readonly String EXACT_EXPIRING_TOKEN = "Exact_Expiring_Token";

        // URLS
        public static string TOKEN_ENDPOINT = "https://start.exactonline.nl/api/oauth2/token";
        public static string USERINFO_ENDPOINT = "https://start.exactonline.nl/api/v1/current/Me";
        public static string AUTHORIZE_ENDPOINT = "https://start.exactonline.nl/api/oauth2/auth";

        // EXACT

        // @hanze
        //public static string CLIENT_ID = "84fbdc70-98e4-42f6-bd1c-c9834d740868";
        //public static string CLIENT_SECRET = "PatUNqa5AgrJ";

        // @live
        //public static string CLIENT_ID = "f676efa2-e88e-407b-bb54-87ac951183e9";
        //public static string CLIENT_SECRET = "ZXSf1wQSfB6V";

        // @INDI
        public static string CLIENT_ID = "d79cb75c-fb29-4469-85d1-227a853336f8";
        public static string CLIENT_SECRET = "iVuaVEGNLqMB";

        // PROVIDER
        public static string PROVIDER_EXACT_ONLINE = "ExactOnline";

        // STATIC DATA
        public static int TEST_INDI_DIVISION_ID = 0;

    }
}
