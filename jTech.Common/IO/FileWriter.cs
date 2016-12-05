using System.ComponentModel.Composition;
using System.IO;

namespace jTech.Common.IO
{
    public interface IFileWriter
    {
        void Write(string filePath, string message);
    }

    [Export(typeof(IFileWriter))]
    public class FileWriter : IFileWriter
    {
        public void Write(string filePath, string message)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine(message);
                }
            }
        }

    }
}
