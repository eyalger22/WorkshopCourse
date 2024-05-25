namespace Market.DataObject
{
    public class Response<T>
    {
        public T? Value { get; }
        public bool HasError { get; }
        public string? ErrorMsg { get; }
        public int? ErrorKind { get; }//0 for success

        public Response(T val)
        {
            Value = val;
            ErrorKind = 0;
            HasError = false;
        }

        public Response(string msg, int errorKind)
        {
            ErrorKind = errorKind;
            ErrorMsg = msg;
            HasError = true;
        }
    }
}
