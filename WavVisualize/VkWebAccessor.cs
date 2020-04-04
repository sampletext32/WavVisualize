using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WavVisualize
{
    internal static class RandomString
    {
        private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_-";

        public static string Generate(int length)
        {
            var bytes = new byte[length];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(bytes);
            }

            var result = new StringBuilder(length);
            foreach (var b in bytes)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }
    }

    public class VkWebAccessor
    {
        public static async Task<string> PostAsync(Uri uri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            #region This is just test

            //if (_logger != null)
            //{
            //    var json = JsonConvert.SerializeObject(parameters);
            //    _logger.LogDebug($"POST request: {uri}{Environment.NewLine}{Utilities.PrettyPrintJson(json)}");
            //}

            #endregion

            #region Do The Post

            //var content = new FormUrlEncodedContent(parameters);

            //MY: Simply do the post request with parameters

            #endregion

            return "";
        }

        public async Task<string> CallMethodAsync(string methodName, Dictionary<string, string> parameters)
        {
            var url = $"https://api.vk.com/method/{methodName}";

            var jObject = await PostAsync(new Uri(url), parameters);

            //Extract Json "response"

            return "" /*response*/;
        }

        public T Perform<T>(Func<ulong?, string, T> action)
        {
            int MaxCaptchaRecognitionCount = 0;
            var numberOfRemainingAttemptsToSolveCaptcha = MaxCaptchaRecognitionCount;
            var numberOfRemainingAttemptsToAuthorize = MaxCaptchaRecognitionCount + 1;
            ulong? captchaSidTemp = null;
            string captchaKeyTemp = null;
            var callCompleted = false;
            var result = action.Invoke(captchaSidTemp, captchaKeyTemp);
            return result;
        }

        public async Task<string> CallUriAsync(Uri uri, Dictionary<string, string> parameters,
            CancellationToken cancellationToken = default)
        {
            if (!parameters.ContainsKey("v"))
            {
                //(version defaults to 5.103 for today)
                parameters.Add("v", "5.103");
            }

            string response = await PostAsync(uri, parameters);

            #region Handle With Captcha

            //var response = await Perform((sid, key) =>
            //{
            //    parameters.Add("captcha_sid", sid?.ToString());
            //    parameters.Add("captcha_key", key);

            //    return PostAsync(uri, parameters);
            //});

            #endregion


            return response;
        }

        public static async Task<string> RefreshTokenAsync(string oldToken, string receipt)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"receipt", receipt},
                {"access_token", oldToken}
            };

            //MY: Call method "auth.refreshToken" with parameters

            //var response = await _vkApiInvoker.CallAsync("auth.refreshToken", parameters).ConfigureAwait(false);

            //MY: Extract "token" from JSON (!token as a new access_token)

            return "" /*token*/;
        }

        private static async Task<string> BaseAuthAsync()
        {
            try
            {
                var parameters = new Dictionary<string, string>()
                {
                    {"grant_type", "password"},
                    {"client_id", "2274003"},
                    {"client_secret", "hHbZxrka2uZ6jB1inYsH"},
                    //default TwoFactorSupported to true
                    //{"2fa_supported", _apiAuthParams.TwoFactorSupported ?? true},
                    {"2fa_supported", true.ToString()},
                    //default ForceSms to false
                    //{"force_sms", _apiAuthParams.ForceSms},
                    {"force_sms", false.ToString()},

                    //TODO: Set login and password
                    //{"username", _apiAuthParams.Login},
                    //{"password", _apiAuthParams.Password},

                    //IMPORTANT: CODE IS CAPTCHA RESOLVED STRING!!!
                    //default Code to 0
                    //{"code", _apiAuthParams.Code},
                    {"code", "0"},
                    {"scope", "all"},

                    //TODO: Save generated string
                    {"device_id", RandomString.Generate(16)}
                };

                return await PostAsync(new Uri("https://oauth.vk.com/token"), parameters);
            }
            catch /*(VkAuthException exception)*/
            {
                #region Handle Captcha

                //switch (exception.AuthError.Error)
                //{
                //    case "need_validation":
                //_logger?.LogDebug("Требуется ввести код двухфакторной авторизации");

                //My: If TwoFactorAuthorization is unsupported - throw

                //if (_apiAuthParams.TwoFactorAuthorization == null)
                //{
                //    throw new
                //        InvalidOperationException(
                //            $"Two-factor authorization required, but {nameof(_apiAuthParams.TwoFactorAuthorization)} callback is null. Set {nameof(_apiAuthParams.TwoFactorAuthorization)} callback to handle two-factor authorization.");
                //}

                //MY: Resolve Captcha And Save To Code

                #endregion

                #region Call BaseAuthAsync With Resolved Code

                //var result = _apiAuthParams.TwoFactorAuthorization();
                //_apiAuthParams.Code = result;

                //        return await BaseAuthAsync().ConfigureAwait(false);
                //    default:
                //        throw;
                //}

                #endregion
            }

            return "";
        }

        public static async void Authorize()
        {
            #region Authorize

            //var authResult = await BaseAuthAsync().ConfigureAwait(false);

            #endregion

            #region SafetyNet Receipt

            //var response = await _safetyNetClient.CheckIn().ConfigureAwait(false);
            //var receipt = await _safetyNetClient.GetReceipt(response).ConfigureAwait(false);

            //receipt = receipt?.Remove(0, 7);

            #endregion

            //MY: If receipt is empty, then we cant proceed next

            //if (string.IsNullOrWhiteSpace(receipt))
            //{
            //    throw new VkApiException("receipt is null or empty");
            //}

            #region Update AccessToken With Receipt

            //var parameters = new Dictionary<string, string>()
            //{
            //    {"receipt", receipt},
            //    {"access_token", token}
            //};

            //MY: Call method "auth.refreshToken" with parameters

            //var response = await _vkApiInvoker.CallAsync("auth.refreshToken", parameters).ConfigureAwait(false);

            //MY: Extract "token" from JSON (!token as a new access_token)

            #endregion

            #region Combine AccessToken And UserId

            //return new
            //{
            //    AccessToken = token,
            //    ExpiresIn = authResult.ExpiresIn,
            //    UserId = authResult.UserId
            //};

            #endregion
        }
    }
}