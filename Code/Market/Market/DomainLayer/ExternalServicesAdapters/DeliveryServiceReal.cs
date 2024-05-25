using Market.DataObject;

namespace Market.DomainLayer.ExternalServicesAdapters
{
    public class DeliveryServiceReal : DeliveryService
    {
        public Response<int> cancel_supply(string id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                var values = new Dictionary<string, string>()
                {
                    {"action_type","cancel_supply" },
                    {"transaction_id", id },
                };
                var content = new FormUrlEncodedContent(values);
                try
                {
                    var response = httpClient.PostAsync("https://php-server-try.000webhostapp.com/", content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        if (int.TryParse(res, out int result))
                        {
                            return new Response<int>(result);
                        }
                        else
                        {
                            return new Response<int>("Failed to parse the result as Integer", 2);
                        }
                    }
                    return new Response<int>($"POST request failed with status code: {response.StatusCode}", 2);
                }
                catch (Exception ex)
                {
                    return new Response<int>(ex.Message, 1);
                }
            }
        }

        public Response<int> createDelivery(DeliveryDetails details)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                var values = new Dictionary<string, string>()
                {
                    {"action_type","supply" },
                    {"name",details.Name },
                    {"address",details.Address },
                    {"city",details.City },
                    {"country",details.Country },
                    {"zip",details.Zip },
                };
                var content = new FormUrlEncodedContent(values);
                try
                {
                    var response = httpClient.PostAsync("https://php-server-try.000webhostapp.com/", content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        if (int.TryParse(res, out int result))
                        {
                            return new Response<int>(result);
                        }
                        else
                        {
                            return new Response<int>("Failed to parse the result as Integer", 2);
                        }
                    }
                    return new Response<int>($"POST request failed with status code: {response.StatusCode}", 2);
                }
                catch (Exception ex)
                {
                    return new Response<int>(ex.Message, 1);
                }
            }
        }

        public Response<string> handshake()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                var values = new Dictionary<string, string>()
                {
                    {"action_type","handshake" },
                };
                var content = new FormUrlEncodedContent(values);
                string res;
                try
                {
                    var response = httpClient.PostAsync("https://php-server-try.000webhostapp.com/", content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return new Response<string>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                    }
                    return new Response<string>($"POST request failed with status code: {response.StatusCode}", 2);
                }
                catch (Exception ex)
                {
                    return new Response<string>(ex.Message, 1);
                }
            }
        }
    }
}
