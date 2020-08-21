using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CompresorImagenes
{
    class ManejadorZip
    {
        private readonly string RUTA_ZIP = "comprimidos.zip";

        public ManejadorZip()
        {
            if (File.Exists(RUTA_ZIP))
                File.Delete(RUTA_ZIP);
            var fs = File.Create(RUTA_ZIP);
            fs.Close();
        }
        
        public async Task AgregarAZipAsync(byte[] datos)
        {
            await Task.Factory.StartNew(() =>
            {
                Comprimir(datos);
            });
        }

        private void Comprimir(byte[] datos)
        {
            using (var os = new FileStream(RUTA_ZIP, FileMode.Open))
            using (var zip = new ZipArchive(os, ZipArchiveMode.Update))
            {
                var entry = zip.CreateEntry($"miniatura_{Guid.NewGuid()}.jpg");
                using (var ms = new MemoryStream(datos))
                using (var entryStream = entry.Open())
                    ms.CopyTo(entryStream);
            }
        }
        
    }
}
