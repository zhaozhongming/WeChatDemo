//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices;
//using System.Text;

//namespace wechat4ap_demo.Classes
//{
//    /// <summary>
//    /// Supports conversion of Adobe PDF formatted data to image formats.
//    /// </summary>
//    public class PdfConvert
//    {
//        #region P/Invoke
//        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
//        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

//        [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
//        private static extern int gsapi_new_instance(out IntPtr pinstance, IntPtr caller_handle);

//        [DllImport("gsdll32.dll", EntryPoint = "gsapi_init_with_args")]
//        private static extern int gsapi_init_with_args(IntPtr instance, int argc, IntPtr argv);

//        [DllImport("gsdll32.dll", EntryPoint = "gsapi_exit")]
//        private static extern int gsapi_exit(IntPtr instance);

//        [DllImport("gsdll32.dll", EntryPoint = "gsapi_delete_instance")]
//        private static extern void gsapi_delete_instance(IntPtr instance);
//        #endregion

//        #region Public Properties
//        /// <summary>
//        /// Gets or sets the destination image format for conversion.
//        /// </summary>
//        /// <remarks>
//        /// The default is 24-bit RGB TIFF.
//        /// </remarks>
//        public PdfImageFormat ImageFormat { get; set; }

//        /// <summary>
//        /// Gets or sets a default destination page size in the absence of specific width and height.
//        /// </summary>
//        /// <remarks>
//        /// The default is letter size.
//        /// </remarks>
//        public PdfPageSize DefaultPageSize { get; set; }

//        /// <summary>
//        /// Gets or sets the destination width of the image in pixels.
//        /// </summary>
//        /// <remarks>
//        /// The default is 0, such that DefaultPageSize will be used instead.
//        /// </remarks>
//        public int Width { get; set; }

//        /// <summary>
//        /// Gets or sets the destination height of the image in pixels.
//        /// </summary>
//        /// <remarks>
//        /// The default is 0, such that DefaultPageSize will be used instead.
//        /// </remarks>
//        public int Height { get; set; }

//        /// <summary>
//        /// Gets or sets the destination horizontal resolution of the image in DPI/PPI.
//        /// </summary>
//        /// <remarks>
//        /// The default is 0, such that a resolution will be automatically chosen.
//        /// </remarks>
//        public int ResolutionX { get; set; }

//        /// <summary>
//        /// Gets or sets the destination vertical resolution of the image in DPI/PPI.
//        /// </summary>
//        /// <remarks>
//        /// The default is 0, such that a resolution will be automatically chosen.
//        /// </remarks>
//        public int ResolutionY { get; set; }

//        /// <summary>
//        /// Gets or sets the first page of the PDF in a range conversion.
//        /// </summary>
//        /// <remarks>
//        /// The default is -1 representing the first page in the file.
//        /// </remarks>
//        public int FirstPage { get; set; }

//        /// <summary>
//        /// Gets or sets the last page of the PDF in a range conversion.
//        /// </summary>
//        /// <remarks>
//        /// The default is -1 representing the last page in the file.
//        /// </remarks>
//        public int LastPage { get; set; }

//        /// <summary>
//        /// Gets or sets the quality of the image when ImageFormat is "jpeg".
//        /// </summary>
//        /// <remarks>
//        /// The default is 75.
//        /// </remarks>
//        public int JpegQuality { get; set; }

//        /// <summary>
//        /// Gets or sets the compression type when ImageFormat is "tiff".
//        /// </summary>
//        /// <remarks>
//        /// The default is uncompressed.
//        /// </remarks>
//        public PdfTiffCompression TiffCompression { get; set; }

//        /// <summary>
//        /// Gets or sets whether images will be fit to the default page size.
//        /// </summary>
//        public bool FitPage { get; set; }

//        /// <summary>
//        /// Gets or sets whether each page is converted to a separate file.
//        /// </summary>
//        /// <remarks>
//        /// This property must be true if ImageFormat does not support multiple pages,
//        /// otherwise only the first page in the source PDF will be converted.
//        /// </remarks>
//        public bool SeparatePages { get; set; }
//        #endregion

//        #region Public Interface
//        /// <summary>
//        /// Creates and initializes a new instance.
//        /// </summary>
//        public PdfConvert()
//        {
//            ImageFormat = PdfImageFormat.tiff24nc;
//            DefaultPageSize = PdfPageSize.letter;
//            FirstPage = -1;
//            LastPage = -1;
//            TiffCompression = PdfTiffCompression.none;
//        }

//        /// <summary>
//        /// Converts the provided PDF represented by a byte array to an image file.
//        /// </summary>
//        /// <param name="input">The source PDF byte array.</param>
//        /// <param name="output">The destination image file.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        public bool Convert(byte[] input, string output)
//        {
//            using (var ms = new MemoryStream(input))
//            {
//                return Convert(ms, output);
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF represented by a byte array to the provided destination stream.
//        /// </summary>
//        /// <param name="input">The source PDF byte array.</param>
//        /// <param name="output">The destination stream.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        /// <remarks>
//        /// If the destination stream is seekable, the position will be set to the beginning.
//        /// </remarks>
//        public bool Convert(byte[] input, Stream output)
//        {
//            using (var ms = new MemoryStream(input))
//            {
//                return Convert(ms, output);
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF file represented by a byte array to a byte array.
//        /// </summary>
//        /// <param name="input">The source PDF byte array.</param>
//        /// <returns>A byte array on successful conversion, or null if the conversion fails.</returns>
//        public byte[] Convert(byte[] input)
//        {
//            using (var ms = new MemoryStream(input))
//            {
//                return Convert(input, ms) ? ms.ToArray() : null;
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF file to a byte array.
//        /// </summary>
//        /// <param name="input">The source PDF file.</param>
//        /// <returns>A byte array on successful conversion, or null if the conversion fails.</returns>
//        public byte[] Convert(string input)
//        {
//            using (var ms = new MemoryStream())
//            {
//                return Convert(input, ms) ? ms.ToArray() : null;
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF file to an image file.
//        /// </summary>
//        /// <param name="input">The source PDF file.</param>
//        /// <param name="output">The destination image file.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        public bool Convert(string input, string output)
//        {
//            return ExecuteGhostscriptCommand(BuildGhostscriptCommand(input, output));
//        }

//        /// <summary>
//        /// Converts the provided PDF file to the provided destination stream.
//        /// </summary>
//        /// <param name="input">The source PDF file.</param>
//        /// <param name="output">The destination stream.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        /// <remarks>
//        /// If the destination stream is seekable, the position will be set to the beginning.
//        /// </remarks>
//        public bool Convert(string input, Stream output)
//        {
//            var dstFile = Path.GetTempFileName();

//            try
//            {
//                // Ghostscript only works with files, so we need to use temporary destination file for conversion.
//                if (ExecuteGhostscriptCommand(BuildGhostscriptCommand(input, dstFile)))
//                {
//                    using (var reader = File.OpenRead(dstFile))
//                    {
//                        reader.CopyTo(output);

//                        // Reset the stream if we can
//                        if (output.CanSeek)
//                        {
//                            output.Seek(0, SeekOrigin.Begin);
//                        }
//                    }

//                    return true;
//                }
//            }
//            finally
//            {
//                if (File.Exists(dstFile))
//                {
//                    File.Delete(dstFile);
//                }
//            }

//            return false;
//        }

//        /// <summary>
//        /// Converts the provided PDF represented by a stream to a byte array.
//        /// </summary>
//        /// <param name="input">The source PDF stream.</param>
//        /// <returns>A byte array on successful conversion, or null if the conversion fails.</returns>
//        /// <remarks>
//        /// The source stream position will be modified by this conversion.
//        /// </remarks>
//        public byte[] Convert(Stream input)
//        {
//            var srcFile = Path.GetTempFileName();

//            try
//            {
//                // Ghostscript only works with files, so we need to send the stream to a temporary file for conversion.
//                using (var writer = new FileStream(srcFile, FileMode.Create, FileAccess.Write))
//                {
//                    input.CopyTo(writer);
//                }

//                return Convert(srcFile);
//            }
//            finally
//            {
//                if (File.Exists(srcFile))
//                {
//                    File.Delete(srcFile);
//                }
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF represented by a stream to an image file.
//        /// </summary>
//        /// <param name="input">The source PDF stream.</param>
//        /// <param name="output">The destination image file.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        /// <remarks>
//        /// The source stream position will be modified by this conversion.
//        /// </remarks>
//        public bool Convert(Stream input, string output)
//        {
//            var srcFile = Path.GetTempFileName();

//            try
//            {
//                // Ghostscript only works with files, so we need to send the stream to a temporary file for conversion.
//                using (var writer = new FileStream(srcFile, FileMode.Create, FileAccess.Write))
//                {
//                    input.CopyTo(writer);
//                }

//                return Convert(srcFile, output);
//            }
//            finally
//            {
//                if (File.Exists(srcFile))
//                {
//                    File.Delete(srcFile);
//                }
//            }
//        }

//        /// <summary>
//        /// Converts the provided PDF represented by a stream to the provided destination stream.
//        /// </summary>
//        /// <param name="input">The source PDF stream.</param>
//        /// <param name="output">The destination stream.</param>
//        /// <returns>True if the conversion succeeded.</returns>
//        /// <remarks>
//        /// The source stream position will be modified by this conversion.
//        /// If the destination stream is seekable, the position will be set to the beginning.
//        /// </remarks>
//        public bool Convert(Stream input, Stream output)
//        {
//            var srcFile = Path.GetTempFileName();

//            try
//            {
//                // Ghostscript only works with files, so we need to send the stream to a temporary file for conversion.
//                using (var writer = new FileStream(srcFile, FileMode.Create, FileAccess.Write))
//                {
//                    input.CopyTo(writer);
//                }

//                return Convert(srcFile, output);
//            }
//            finally
//            {
//                if (File.Exists(srcFile))
//                {
//                    File.Delete(srcFile);
//                }
//            }
//        }
//        #endregion

//        #region Ghostscript Workers
//        /// <summary>
//        /// Executes a generated Ghostscript command for conversion.
//        /// </summary>
//        /// <param name="args">A list of Ghostscript arguments.</param>
//        /// <returns>True of the conversion succeeded, false otherwise.</returns>
//        private bool ExecuteGhostscriptCommand(List<string> args)
//        {
//            var handles = new GCHandle[args.Count];
//            var pHandles = new IntPtr[args.Count];

//            // Ghostscript is a C-based API, so we must allocate fixed memory and convert C# strings to C-style strings
//            for (int i = 0; i < args.Count; i++)
//            {
//                handles[i] = GCHandle.Alloc(Encoding.Default.GetBytes(args[i] != null ? args[i] : string.Empty), GCHandleType.Pinned);
//                pHandles[i] = handles[i].AddrOfPinnedObject();
//            }

//            var memHandle = GCHandle.Alloc(pHandles, GCHandleType.Pinned);
//            var pInstance = IntPtr.Zero;
//            var ret = -1;

//            try
//            {
//                ret = gsapi_new_instance(out pInstance, IntPtr.Zero);

//                if (ret >= 0)
//                {
//                    ret = gsapi_init_with_args(pInstance, args.Count, memHandle.AddrOfPinnedObject());
//                }
//            }
//            finally
//            {
//                // Don't forget to release memory, we're in old school coding mode here. :)
//                for (int i = 0; i < handles.Length; i++)
//                {
//                    handles[i].Free();
//                }

//                memHandle.Free();

//                // Safely dispose of Ghostscript
//                if (pInstance != IntPtr.Zero)
//                {
//                    gsapi_exit(pInstance);
//                    gsapi_delete_instance(pInstance);
//                }
//            }

//            return (ret == 0) || (ret == -101);
//        }

//        /// <summary>
//        /// Generates a list of Ghostscript arguments based on current property settings.
//        /// </summary>
//        /// <param name="inputFile">The source PDF file for conversion.</param>
//        /// <param name="outputFile">The destination image file for conversion.</param>
//        /// <returns>A list of constructed Ghostscript arguments.</returns>
//        private List<string> BuildGhostscriptCommand(string inputFile, string outputFile)
//        {
//            var args = new List<string>();

//            args.Add("pdf2img");
//            args.Add("-dNOPAUSE");
//            args.Add("-dBATCH");
//            args.Add("-dSAFER");
//            args.Add(string.Format("-sDEVICE={0}", ImageFormat));

//            // If an explicit size is specified, use that. Otherwise use the default page size.
//            if (Width > 0 && Height > 0)
//            {
//                args.Add(string.Format("-g{0}x{1}", Width, Height));
//            }
//            else
//            {
//                args.Add(string.Format("-sPAPERSIZE={0}", DefaultPageSize));
//            }

//            if (ResolutionX > 0 && ResolutionY > 0)
//            {
//                args.Add(string.Format("-r{0}x{1}", ResolutionX, ResolutionY));
//            }

//            if (FirstPage > 0)
//            {
//                args.Add(string.Format("-dFirstPage={0}", FirstPage));
//            }

//            if (LastPage > 0)
//            {
//                args.Add(string.Format("-dLastPage={0}", LastPage));
//            }

//            // Apply format-specific options
//            if (ImageFormat == PdfImageFormat.jpeg && JpegQuality > 0 && JpegQuality < 101)
//            {
//                args.Add(string.Format("-dJPEGQ={0}", JpegQuality));
//            }
//            else if (ImageFormat.ToString().StartsWith("tiff"))
//            {
//                args.Add(string.Format("-sCompression={0}", TiffCompression));
//            }

//            if (FitPage)
//            {
//                args.Add("-dFitPage");
//            }

//            if (SeparatePages)
//            {
//                // Format the output file such that Ghostscript can version more than one
//                int lastDotIndex = outputFile.LastIndexOf('.');

//                if (lastDotIndex > 0)
//                {
//                    // Note: Ghostscript uses a printf-like string format for versioned file names
//                    outputFile = outputFile.Insert(lastDotIndex, "%04d");
//                }
//            }

//            args.Add(string.Format("-sOutputFile={0}", outputFile));
//            args.Add(inputFile);

//            return args;
//        }
//        #endregion
//    }
//}