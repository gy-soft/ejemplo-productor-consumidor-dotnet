using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CompresorImagenes
{
    class ManejadorImagenes
    {
        private readonly int ANCHO = 300;
        private readonly int ALTO = 200;

        public IEnumerable<string> GetListadoFotografias()
        {
            var dir = Directory.CreateDirectory("fotos");
            FileInfo[] fileInfo = dir.GetFiles();
            return fileInfo.Select(i => i.FullName);
        }
        public async Task<byte[]> GenerarMiniaturaAsync(string rutaFoto)
        {
            return await Task.Factory.StartNew(() =>
            {
                var foto = Image.FromFile(rutaFoto);
                var original = new Bitmap(foto);
                var miniatura = new Bitmap(ANCHO, ALTO);
                using (Graphics g = Graphics.FromImage(miniatura))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.DrawImage(original, new Rectangle(0, 0, ANCHO, ALTO));
                }
                original.Dispose();

                byte[] raw;
                using (var ms = new MemoryStream())
                {
                    miniatura.Save(ms, ImageFormat.Jpeg);
                    raw = ms.ToArray();
                }
                return raw;
            });
            
        }
    }
}
