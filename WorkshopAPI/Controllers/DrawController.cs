using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WorkshopAPI.Models;

namespace WorkshopAPI.Controllers
{
    [RoutePrefix("api")]
    public class DrawController : ApiController
    {
        [Route("draw")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostDrawImage()
        {
            /* Dese */
            MyDataType mydata = JsonConvert.DeserializeObject<MyDataType>(await Request.Content.ReadAsStringAsync());           
            Pen pen = new Pen(Brushes.Tomato, 5);
            
            HttpClient httpClient = new HttpClient();            

            try
            {
                /* Get image form internet */
                HttpResponseMessage getMessage = await httpClient.GetAsync(mydata.imageuri);
                string imageType = getMessage.Content.Headers.ContentType.MediaType;
                Stream originImage = await getMessage.Content.ReadAsStreamAsync();

                /* Draw the rectangle where the face is */
                Image drawedImage = Image.FromStream(originImage);                

                var graphic = Graphics.FromImage(drawedImage);
                for (int i = 0; i < mydata.ages.Count; i++)
                {
                    int x = mydata.rects[i].x;
                    int y = mydata.rects[i].y;
                    int len = mydata.rects[i].len;

                    graphic.DrawRectangle(pen, x, y, len, len);
                }

                /* Convert image to base64 string */
                MemoryStream ms = new MemoryStream();

                if (imageType.Equals("image/jpeg"))
                {
                    drawedImage.Save(ms, ImageFormat.Jpeg);
                }
                else if (imageType.Equals("image/png"))
                {
                    drawedImage.Save(ms, ImageFormat.Png);
                }
                else
                {
                    throw new UnsupportedTypeException();
                }
                
                byte[] bytedImage = ms.ToArray();
                string basedImage = Convert.ToBase64String(bytedImage);

                /* Return base64 string */
                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(basedImage);

                #region Test function store image file
                //using (var imageFile = new FileStream(root + "girl.jpg", FileMode.Create))
                //{
                //    imageFile.Write(bytedImage, 0, bytedImage.Length);                    
                //    imageFile.Flush();                    
                //}
                #endregion

                return response;
            }
            catch (GetPictureException gpe)
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, gpe);
                return response;
            }
            catch (UnsupportedMediaTypeException ust)
            {
                var response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ust);
                return response;
            }
            catch (Exception ex)
            {
                var request = Request.CreateErrorResponse(HttpStatusCode.RequestTimeout, ex);
                return request;
            }
        }
    }

    public class GetPictureException : Exception
    {
        public GetPictureException()
            : base("Cannot get the picture, check your url") { }
    }

    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException()
            : base("Makesure your image type is JPEG or PNG") { }
    }
}
