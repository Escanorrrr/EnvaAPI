namespace EnvaTest.Result
{
    public class Result<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public static Result<T> Success(T data, string message = "İşlem başarılı")
        {
            return new Result<T>
            {
                StatusCode = 200,
                Data = data,
                Message = message
            };
        }

        public static Result<T> Error(string message = "Bir hata oluştu", int statusCode = 400)
        {
            return new Result<T>
            {
                StatusCode = statusCode,
                Data = default,
                Message = message
            };
        }

        public static Result<T> NotFound(string message = "Kayıt bulunamadı")
        {
            return new Result<T>
            {
                StatusCode = 404,
                Data = default,
                Message = message
            };
        }
    }

    // Data olmadan kullanılacak durumlar için
    public class Result
    {
        public int StatusCode { get; set; }
        public bool Data { get; set; }
        public string Message { get; set; }

        public static Result Success(string message = "İşlem başarılı")
        {
            return new Result
            {
                StatusCode = 200,
                Data = true,
                Message = message
            };
        }

        public static Result Error(string message = "Bir hata oluştu", int statusCode = 400)
        {
            return new Result
            {
                StatusCode = statusCode,
                Data = false,
                Message = message
            };
        }
    }
} 