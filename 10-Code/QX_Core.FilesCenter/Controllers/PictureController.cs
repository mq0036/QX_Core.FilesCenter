using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using QX_Core.FilesCenter.QX_Core.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QX_Core.FilesCenter.Controllers
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PicturesController : Controller
    {
        private IHostingEnvironment hostingEnv;

        string[] pictureFormatArray = { "png", "jpg", "jpeg", "bmp", "gif","ico", "PNG", "JPG", "JPEG", "BMP", "GIF","ICO" };

        public PicturesController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }

        [HttpPost]
        public IActionResult Post()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);

            //size > 100MB refuse upload !
            if (size > 104857600)
            {
                return Json(Return_Helper_DG.Error_Msg_Ecode_Elevel_HttpCode("pictures total size > 100MB , server refused !"));
            }

            List<string> filePathResultList = new List<string>();

            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string filePath = hostingEnv.WebRootPath + $@"\Files\Pictures\";

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string suffix = fileName.Split('.')[1];

                if (!pictureFormatArray.Contains(suffix))
                {
                    return Json(Return_Helper_DG.Error_Msg_Ecode_Elevel_HttpCode("the picture format not support ! you must upload files that suffix like 'png','jpg','jpeg','bmp','gif','ico'."));
                }

                fileName = Guid.NewGuid() + "." + suffix;

                string fileFullName = filePath + fileName;

                using (FileStream fs = System.IO.File.Create(fileFullName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                filePathResultList.Add($"/src/Pictures/{fileName}");
            }

            string message = $"{files.Count} file(s) /{size} bytes uploaded successfully!";

            return Json(Return_Helper_DG.Success_Msg_Data_DCount_HttpCode(message, filePathResultList, filePathResultList.Count));
        }

    }
}

