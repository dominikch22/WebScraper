using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class LocalHttpServer
    {
        public static void Start()
        {
            // Ścieżka do folderu, który ma być udostępniony przez serwer
            string folderPath = @"C:\webscraper\alamakota.pl\";

            // Adres URL serwera
            string serverUrl = "http://localhost:8080/";

            // Utwórz nowy obiekt HttpListener
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(serverUrl);

            try
            {
                // Rozpocznij nasłuchiwanie żądań
                listener.Start();
                Console.WriteLine("Server started. Listening for requests...");

                // Nasłuchuj żądania
                while (true)
                {
                    // Oczekuj na żądanie
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    // Pobierz ścieżkę z żądania
                    string requestUrl = request.Url.AbsolutePath;
                    string filePath = folderPath + requestUrl.Replace("/", "\\");

                    // Sprawdź, czy plik istnieje
                    if (File.Exists(filePath))
                    {
                        // Odczytaj zawartość pliku
                        byte[] fileBytes = File.ReadAllBytes(filePath);

                        // Ustaw nagłówki odpowiedzi
                        string contentType;
                        if (filePath.EndsWith(".html") || filePath.EndsWith(".htm"))
                        {
                            contentType = "text/html";
                        }
                        else if (filePath.EndsWith(".css"))
                        {
                            contentType = "text/css";
                        }
                        else if (filePath.EndsWith(".svg"))
                        {
                            contentType = "image/svg+xml";
                        }
                        else if (filePath.EndsWith(".min"))
                        {
                            // Załóżmy, że .min to pliki fontów, użyj application/font-woff
                            contentType = "application/font-woff";
                        }
                        else
                        {
                            contentType = "application/octet-stream";
                        }

                        response.ContentType = contentType;
                        response.ContentLength64 = fileBytes.Length;

                        // Wyślij zawartość pliku w odpowiedzi
                        response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
                        response.OutputStream.Close();
                    }
                    else
                    {
                        // Jeśli plik nie istnieje, zwróć 404 Not Found
                        response.StatusCode = 404;
                        response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Zatrzymaj nasłuchiwanie po zakończeniu programu
                listener.Stop();
            }
        }
    }
}
