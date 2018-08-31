using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CamundaClient.Dto
{
    public class FileParameter
    {
        public byte[] File { get; }
        public string FileName { get; }
        public string ContentType { get; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }

        private static string _GetFileNameFromResourceName(string resourceName)
        {
            // NOTE: this code assumes that all of the file names have exactly one
            // extension separator (i.e. "dot"/"period" character). I.e. all file names
            // do have an extension, and no file name has more than one extension.
            // Directory names may have periods in them, as the compiler will escape these
            // by putting an underscore character after them (a single character
            // after any contiguous sequence of dots). IMPORTANT: the escaping
            // is not very sophisticated; do not create folder names with leading
            // underscores, otherwise the dot separating that folder from the previous
            // one will appear to be just an escaped dot!
            bool isWindows = false;
            string windir = Environment.GetEnvironmentVariable("windir");
            if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
            {
                isWindows = true;
            }
            StringBuilder sb = new StringBuilder();
            bool escapeDot = false, haveExtension = false;

            for (int i = resourceName.Length - 1; i >= 0 ; i--)
            {
                if (resourceName[i] == '_')
                {
                    escapeDot = true;
                    continue;
                }

                if (resourceName[i] == '.')
                {
                    if (!escapeDot)
                    {
                        if (haveExtension)
                        {
                            if (!isWindows) 
                                sb.Append('/');
                            else
                                sb.Append('\\');
                            continue;
                        }
                        haveExtension = true;
                    }
                }
                else
                {
                    escapeDot = false;
                }

                sb.Append(resourceName[i]);
            }

            string fileName = Path.GetDirectoryName(sb.ToString());

            fileName = new string(fileName.Reverse().ToArray());

            return System.IO.Path.GetFileName(fileName);
        }

        public static FileParameter FromManifestResource(Assembly assembly, string resourcePath)
        {
            Stream resourceAsStream = assembly.GetManifestResourceStream(resourcePath);
            byte[] resourceAsBytearray;
            using (MemoryStream ms = new MemoryStream())
            {
                resourceAsStream.CopyTo(ms);
                resourceAsBytearray = ms.ToArray();
            }

            // TODO: Verify if this is the correct way of doing it:
            // string assemblyBaseName = assembly.GetName().Name;
            // string fileLocalName = resourcePath.Replace(assemblyBaseName + ".", "");

            return new FileParameter(resourceAsBytearray, _GetFileNameFromResourceName(resourcePath));
        }
    }
}
