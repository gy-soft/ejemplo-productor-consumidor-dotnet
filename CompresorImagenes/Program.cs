using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CompresorImagenes
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generar miniaturas y comprimir");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            var manejadorZip = new ManejadorZip();
            var manejadorImagenes = new ManejadorImagenes();
            IEnumerable<string> listaDeFotografias = manejadorImagenes.GetListadoFotografias();

            Channel<byte[]> m_channel = Channel.CreateUnbounded<byte[]>();

            ChannelReader<byte[]> reader = m_channel.Reader;
            await Task.Factory.StartNew(async () => {

                while (await reader.WaitToReadAsync())
                    while (reader.TryRead(out byte[] miniatura))
                      await manejadorZip.AgregarAZipAsync(miniatura);
            });

            ChannelWriter<byte[]> writer = m_channel.Writer;
            foreach (var foto in listaDeFotografias)
            {
                // Generar miniatura
                byte[] miniatura = await manejadorImagenes.GenerarMiniaturaAsync(foto);
                writer.TryWrite(miniatura);
            }
            writer.Complete();
        }
    }
}
