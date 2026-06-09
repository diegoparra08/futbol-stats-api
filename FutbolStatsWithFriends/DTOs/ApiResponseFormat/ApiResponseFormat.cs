public class ApiResponseFormat<T>
{
    public bool Succeeded { get; set; } //Dara un mensaje de exito True o false error
    public string Message { get; set; }
    public T Data { get; set; } // Aquí puede viajar un jugador, un rating, una lista, o null

    // Constructor para respuestas exitosas cuando se deben devolver datos
    public ApiResponseFormat(T data, string message = null)
    {
        Succeeded = true;
        Message = message;
        Data = data;
    }

    // Constructor para mensajes simples o errores sin datos
    public ApiResponseFormat(string message, bool succeeded)
    {
        Succeeded = succeeded;
        Message = message;
        Data = default;
    }
}