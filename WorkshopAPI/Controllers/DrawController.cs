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
            try
            {
                /* Dese */
                MyDataType mydata = JsonConvert.DeserializeObject<MyDataType>(await Request.Content.ReadAsStringAsync());

                /* Get image form internet */
                WebClient wc = new WebClient();
                var originImage = wc.OpenRead(mydata.imageuri);

                /* Draw the rectangle where the face is */
                Image drawedImage = Image.FromStream(originImage);
                Pen pen = new Pen(Brushes.Tomato, 5);
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
                drawedImage.Save(ms, ImageFormat.Jpeg);
                byte[] bytedImage = ms.ToArray();
                string basedImage = Convert.ToBase64String(bytedImage);

                /* Return base64 string */
                HttpResponseMessage response = Request.CreateResponse();
                response.Content = new StringContent(basedImage);

                // Test function store image file
                //using (var imageFile = new FileStream(root + "girl.jpg", FileMode.Create))
                //{
                //    imageFile.Write(bytedImage, 0, bytedImage.Length);                    
                //    imageFile.Flush();                    
                //}
                return response;
            }
            catch (Exception ex)
            {
                var response = Request.CreateResponse();
                response.Content = new StringContent(ex.ToString());
                throw new HttpResponseException(response);
            }
        }
    }    
}
