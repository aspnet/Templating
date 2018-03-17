// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Templates.Test
{
    public class ByteOrderMarkTest
    {
        private readonly ITestOutputHelper _output;

        public ByteOrderMarkTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("\\Microsoft.AspNetCore.SpaTemplates\\content")]
        [InlineData("\\Microsoft.DotNet.Web.ProjectTemplates\\content")]
        [InlineData("\\Microsoft.DotNet.Web.Spa.ProjectTemplates\\content")]
        public void CheckForByteOrderMarkSpaTemplates(string path)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var srcDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\..\..\..\src"));
            var directories = Directory.GetDirectories(srcDirectory + path, "*Sharp");
            var checkFiles = false;
            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory, "*.json");
                foreach (var file in files)
                {
                    var filePath = Path.GetFullPath(file);
                    var fileStream = new FileStream(filePath, FileMode.Open);

                    var bits = new byte[3];
                    fileStream.Read(bits, 0, 3);

                    // Check for UTF8 BOM 0xEF,0xBB,0xBF
                    if (bits[0] == 0xEF && bits[1] == 0xBB && bits[2] == 0xBF)
                    {
                        _output.WriteLine($"File {filePath} has UTF-8 BOM characters.");
                        checkFiles = true;
                    }
                    // Check for UTF16 BOM 0xFF, 0xFE
                    if (bits[0] == 0xFF && bits[1] == 0xFE)
                    {
                        _output.WriteLine($"File {filePath} has UTF-16 BOM characters.");
                        checkFiles = true;
                    }
                }
            }

            Assert.False(checkFiles);
        }
    }
}
