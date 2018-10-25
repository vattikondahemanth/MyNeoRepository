using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateAccountStatementDLL
{
    public class PDFEncryptor
    {
        public MemoryStream PasswordProtectPDF(byte[] input,  MemoryStream output,string password)
        {
            PdfReader reader = new PdfReader(input);
            PdfEncryptor.Encrypt(reader, output, true, password, password, PdfWriter.AllowModifyContents);
            return output;
        }
        
    }
}
