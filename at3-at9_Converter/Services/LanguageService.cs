namespace at3_at9_Converter.Services
{
    public class LanguageService
    {
        public string GetConversionTypeLabel(bool isSpanish)
        {
            return isSpanish ? "Tipo de Conversión" : "Conversion Type";
        }

        public string GetBitrateLabel(bool isSpanish)
        {
            return isSpanish ? "Bitrate:" : "Bitrate:";
        }

        public string GetConsoleTypeLabel(bool isSpanish)
        {
            return isSpanish ? "Tipo Consola:" : "Console Type:";
        }

        public string GetDragDropLabel(bool isSpanish)
        {
            return isSpanish ? "Arrastra y suelta tu archivo aquí" : "Drag and drop your file here";
        }

        public string GetConvertButtonLabel(bool isSpanish)
        {
            return isSpanish ? "Convertir" : "Convert";
        }

        public string GetStopPlaybackLabel(bool isSpanish)
        {
            return isSpanish ? "Detener Reproducción" : "Stop Playback";
        }

        public string GetAt9TabLabel(bool isSpanish)
        {
            return isSpanish ? "Conversión AT9" : "AT9 Conversion";
        }

        public string GetAt3TabLabel(bool isSpanish)
        {
            return isSpanish ? "Conversión AT3" : "AT3 Conversion";
        }

        public string GetBitrateTooltip(bool isSpanish)
        {
            return isSpanish ? "Elige el tipo de bitrate" : "Choose the bitrate type";
        }

        public string GetConsoleTooltip(bool isSpanish)
        {
            return isSpanish ? "Elige el tipo de consola" : "Choose the console type";
        }

        public string GetConversionSuccessMessage(bool isSpanish)
        {
            return isSpanish ? "Conversión completada con éxito" : "Conversion completed successfully";
        }

        public string GetConversionErrorMessage(bool isSpanish)
        {
            return isSpanish ? "Error en la conversión" : "Conversion error";
        }

        public string GetFileNotFoundError(bool isSpanish)
        {
            return isSpanish ? "Archivo no encontrado" : "File not found";
        }

        public string GetOverwriteConfirmation(bool isSpanish, string fileName)
        {
            return isSpanish 
                ? $"El archivo '{fileName}' ya existe. ¿Deseas sobrescribirlo?" 
                : $"The file '{fileName}' already exists. Do you want to overwrite it?";
        }

        public string GetRenamePrompt(bool isSpanish, string originalName)
        {
            return isSpanish 
                ? $"El archivo '{originalName}' ya existe. Introduce un nuevo nombre:" 
                : $"The file '{originalName}' already exists. Enter a new name:";
        }
    }
}
