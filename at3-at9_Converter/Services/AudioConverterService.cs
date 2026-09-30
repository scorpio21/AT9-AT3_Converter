using System;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;
using NAudio;

namespace at3_at9_Converter.Services
{
    public class AudioConverterService
    {
        private readonly FileManager _fileManager;
        private readonly BitrateConfig _bitrateConfig;

        public Action<string> StatusUpdate { get; set; }
        public Action<string> LogError { get; set; }

        public AudioConverterService(FileManager fileManager, BitrateConfig bitrateConfig)
        {
            _fileManager = fileManager;
            _bitrateConfig = bitrateConfig;
        }

        public void ConvertWavToAt9(string inputPath, string outputPath, string at9ToolPath, string bitrate)
        {
            string wavToProcess = inputPath;
            bool isTempWav = false;

            try
            {
                using (var reader = new WaveFileReader(inputPath))
                {
                    if (reader.WaveFormat.SampleRate != 48000)
                    {
                        StatusUpdate?.Invoke("Resampling WAV to 48000Hz...");
                        wavToProcess = Path.Combine(Path.GetDirectoryName(inputPath), "temp_normalized.wav");
                        using (var resampler = new WaveFormatConversionStream(new WaveFormat(48000, 16, reader.WaveFormat.Channels), reader))
                        {
                            WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                        }
                        isTempWav = true;
                    }
                }

                StatusUpdate?.Invoke("AT9 in Progress...");
                RunExternalProcess(@"ATRAC\" + at9ToolPath, " -e -br " + bitrate + " -wholeloop \"" + wavToProcess + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke("Error en pre-procesamiento WAV: " + ex.Message);
                throw;
            }
            finally
            {
                if (isTempWav && _fileManager.FileExists(wavToProcess))
                {
                    _fileManager.DeleteFile(wavToProcess);
                }
            }
        }

        public void ConvertAt9ToWav(string inputPath, string outputPath, string at9ToolPath)
        {
            try
            {
                StatusUpdate?.Invoke("Wav in Progress...");
                RunExternalProcess(@"ATRAC\" + at9ToolPath, " -d \"" + inputPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        public void ConvertMp3ToAt9(string inputPath, string outputPath, string tempWavPath, string at9ToolPath, string bitrate)
        {
            try
            {
                StatusUpdate?.Invoke("Wav in Progress...");
                using (Mp3FileReader mp3 = new Mp3FileReader(inputPath))
                {
                    using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(48000, 16, mp3.WaveFormat.Channels), mp3))
                    {
                        WaveFileWriter.CreateWaveFile(tempWavPath, pcm);
                    }
                }

                StatusUpdate?.Invoke("AT9 in Progress...");
                RunExternalProcess(@"ATRAC\" + at9ToolPath, " -e -br " + bitrate + " -wholeloop \"" + tempWavPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        public void ConvertAt9ToMp3(string inputPath, string tempWavPath, string outputPath, string at9ToolPath)
        {
            try
            {
                RunExternalProcess(@"ATRAC\" + at9ToolPath, " -d \"" + inputPath + "\" \"" + tempWavPath + "\"");
                RunExternalProcess(@"LAME\lame.exe", "-V2 \"" + tempWavPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        public void ConvertWavToAt3(string inputPath, string outputPath, string at3ToolPath, string bitrate, string console)
        {
            string wavToProcess = inputPath;
            bool isTempWav = false;

            try
            {
                int targetRate = (console == "PSP") ? 44100 : 48000;
                int targetChannels = (console == "PSP") ? 2 : 0;

                using (var reader = new WaveFileReader(inputPath))
                {
                    if (reader.WaveFormat.SampleRate != targetRate || (targetChannels == 2 && reader.WaveFormat.Channels != 2))
                    {
                        StatusUpdate?.Invoke("Normalizing for PSP (44100Hz Stereo)...");
                        wavToProcess = Path.Combine(Path.GetDirectoryName(inputPath), "temp_psp_norm.wav");
                        var outFormat = new WaveFormat(targetRate, 16, (targetChannels == 2) ? 2 : reader.WaveFormat.Channels);
                        using (var resampler = new WaveFormatConversionStream(outFormat, reader))
                        {
                            WaveFileWriter.CreateWaveFile(wavToProcess, resampler);
                        }
                        isTempWav = true;
                    }
                }

                StatusUpdate?.Invoke("AT3 in Progress...");
                RunExternalProcess(@"ATRAC\" + at3ToolPath, " -e -br " + bitrate + " -wholeloop \"" + wavToProcess + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke("Error PSP: " + ex.Message);
                throw;
            }
            finally
            {
                if (isTempWav && _fileManager.FileExists(wavToProcess))
                {
                    _fileManager.DeleteFile(wavToProcess);
                }
            }
        }

        public void ConvertAt3ToWav(string inputPath, string outputPath, string at3ToolPath)
        {
            try
            {
                RunExternalProcess(@"ATRAC\" + at3ToolPath, " -d \"" + inputPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        public void ConvertMp3ToAt3(string inputPath, string outputPath, string tempWavPath, string at3ToolPath, string bitrate, string console)
        {
            try
            {
                int targetRate = (console == "PSP") ? 44100 : 48000;
                using (Mp3FileReader mp3 = new Mp3FileReader(inputPath))
                {
                    using (WaveStream pcm = new WaveFormatConversionStream(new WaveFormat(targetRate, 16, mp3.WaveFormat.Channels), mp3))
                    {
                        WaveFileWriter.CreateWaveFile(tempWavPath, pcm);
                    }
                }
                RunExternalProcess(@"ATRAC\" + at3ToolPath, " -e -br " + bitrate + " -wholeloop \"" + tempWavPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        public void ConvertAt3ToMp3(string inputPath, string tempWavPath, string outputPath, string at3ToolPath)
        {
            try
            {
                RunExternalProcess(@"ATRAC\" + at3ToolPath, " -d \"" + inputPath + "\" \"" + tempWavPath + "\"");
                RunExternalProcess(@"LAME\lame.exe", "-V2 \"" + tempWavPath + "\" \"" + outputPath + "\"");
            }
            catch (Exception ex)
            {
                LogError?.Invoke(ex.Message);
                throw;
            }
        }

        private void RunExternalProcess(string toolPath, string arguments)
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(appDir, toolPath);

            if (!_fileManager.FileExists(fullPath))
            {
                throw new FileNotFoundException($"Tool not found: {fullPath}");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fullPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = appDir
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    string logPath = Path.Combine(appDir, "conversion_errors.log");
                    File.AppendAllText(logPath, $"\r\n[{DateTime.Now}] Tool: {toolPath}, ExitCode: {process.ExitCode}");
                    File.AppendAllText(logPath, $"\r\nOutput: {output}");
                    File.AppendAllText(logPath, $"\r\nError: {error}\r\n");

                    StatusUpdate?.Invoke($"Error: Tool exited with code {process.ExitCode}");
                }
            }
        }
    }
}
