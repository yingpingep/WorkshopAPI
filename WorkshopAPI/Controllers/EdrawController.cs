using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WorkshopAPI.Controllers
{
    [RoutePrefix("api")]
    public class EdrawController : ApiController
    {
        [Route("edraw")]
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
                Pen pen = new Pen(Brushes.DeepPink, 5);
                Font drawFont = new Font("Arial", 28);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                var graphic = Graphics.FromImage(drawedImage);

                for (int i = 0; i < mydata.rects.Count; i++)
                {
                    int x = mydata.rects[i].x;
                    int y = mydata.rects[i].y;
                    int len = mydata.rects[i].len;
                    
                    graphic.DrawRectangle(pen, x, y, len, len);
                    RectangleF layoutRect = new RectangleF(x - 2, y - 50, 200, 50);
                    graphic.FillRectangle(new SolidBrush(Color.DeepPink), layoutRect);
                    graphic.DrawString(mydata.emoes[i], drawFont, drawBrush, layoutRect);
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

        public class MyDataType
        {
            public string imageuri { get; set; }
            public List<Rect> rects { get; set; }
            public List<string> emoes { get; set; }
        }

        public class Rect
        {
            public int x { get; set; }
            public int y { get; set; }
            public int len { get; set; }
        }        
    }
}
