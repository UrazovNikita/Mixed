using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mixed.Models
{
    public class ImageSetter
    {
        public void SetImage(dynamic model, ref User dataObj)
        {
            if (model.Image != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Image.Length);
                }
                dataObj.Image = imageData;
            }
            else
            {
                byte[] imageData = System.IO.File.ReadAllBytes("wwwroot/images/no_photo.jpg");
                dataObj.Image = imageData;
            }
        }
        public void SetImage(dynamic model, ref Item dataObj)
        {
            if (model.Image != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Image.Length);
                }
                dataObj.Image = imageData;
            }
            else
            {
                byte[] imageData = System.IO.File.ReadAllBytes("wwwroot/images/no_photo.jpg");
                dataObj.Image = imageData;
            }
        }
    }
}
