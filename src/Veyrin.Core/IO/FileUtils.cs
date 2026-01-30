

public class FileUtils
{
    private const int MaxRetries = 3;
    private const int DelayMilliseconds = 500; // Wait 500ms between attempts
    private const int SharingViolationHResult = unchecked((int)0x80070020);

    /// <summary>
    /// 取得指定目錄中唯一的檔案名稱。
    /// </summary>
    /// <param name="directoryPath">檔案所在目錄的完整路徑。</param>
    /// <returns>檔案名稱字串。</returns>
    public static string GetFileName(string directoryPath) => GetUniqueFileContentAndName(directoryPath).FileName;

    /// <summary>
    /// 取得指定目錄中的檔案名稱，支援篩選與子目錄搜尋，並自動跳過權限不足的資料夾。
    /// </summary>
    public static IEnumerable<string> GetFileNames(string directoryPath, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (!Directory.Exists(directoryPath)) return [];
        // 設定列舉選項，確保遇到權限問題或系統錯誤時不會崩潰
        var options = new EnumerationOptions
        {
            // 將 SearchOption 轉換為對應的列舉層級
            RecurseSubdirectories = searchOption == SearchOption.AllDirectories,
            // 關鍵：忽略權限不足的資料夾 (如 System Volume Information)
            IgnoreInaccessible = true,
            // 視需求決定是否包含隱藏檔或系統檔
            AttributesToSkip = FileAttributes.System
        };
        return Directory.EnumerateFiles(directoryPath, searchPattern, options).Select(path => Path.GetFileName(path));
    }

    /// <summary>
    /// 取得指定目錄中唯一的檔案名稱和內容（位元組陣列）。
    /// 假設目錄中只包含一個檔案。
    /// </summary>
    /// <param name="directoryPath">目錄的完整路徑。</param>
    /// <returns>包含檔案名稱和位元組內容的 Tuple。</returns>
    /// <exception cref="InvalidOperationException">當目錄中包含多於一個檔案時拋出。</exception>
    public static (string FileName, byte[] FileBytes) GetUniqueFileContentAndName(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        // 1. 檢查目錄是否存在（維持原行為：不存在則返回空）
        if (!Directory.Exists(directoryPath))
            return ("", []);

        //var fullPath = Path.Combine(_rootPath, path);
        var files = Directory.EnumerateFiles(directoryPath).Take(2).ToList();

        if (files.Count == 0)
            return ("", []);

        //throw new FileNotFoundException($"No files found in directory: {fullPath}");
        if (files.Count > 1)
        {
            throw new InvalidOperationException(
                $"Directory is expected to contain exactly one file, but found {files.Count} files in: {directoryPath}");
        }

        var firstFile = files[0];

        // 4. 文件名處理：
        var fileName = Path.GetFileName(firstFile);

        // 5. 使用 File.ReadAllBytes 讀取，保持原樣
        var fileBytes = File.ReadAllBytes(firstFile);

        return (fileName, fileBytes);
    }


    public static IEnumerable<(string FileName, byte[] FileBytes)> GetAllFileContentsAndNames(string directoryPath)
    {
        // 1. 參數檢查
        if (string.IsNullOrWhiteSpace(directoryPath))
            throw new ArgumentException("Directory path cannot be null or empty.", nameof(directoryPath));

        // 4. 初始化結果列表
        var fileContentsList = new List<(string FileName, byte[] FileBytes)>();

        // 2. 檢查目錄是否存在
        // 保持與原邏輯一致：如果目錄不存在，返回一個空的列表
        if (!Directory.Exists(directoryPath)) return fileContentsList;

        // 3. 獲取目錄內所有文件的完整路徑
        // EnumerateFiles() 比 GetFiles() 在處理大目錄時更高效，因為它使用延遲加載 (lazy loading)
        var allFiles = Directory.EnumerateFiles(directoryPath);

        // 5. 逐一讀取每個文件的內容
        foreach (var fullFilePath in allFiles)
        {
            try
            {
                // 如果目錄不存在就創建
                var fileName = Path.GetFileName(fullFilePath);

                // 讀取文件內容為 byte[]
                var fileBytes = File.ReadAllBytes(fullFilePath);

                // 加入結果列表
                fileContentsList.Add((fileName, fileBytes));
            }
            catch (IOException ex)
            {
                // 處理文件可能正在被其他程序佔用等 I/O 錯誤。
                // 根據您的業務需求，您可以選擇：
                // A. 記錄錯誤並跳過該文件 (Log and continue)
                // B. 拋出異常並中止操作 (Throw and stop) - 這裡選擇 A
                throw new IOException($"Warning: Could not read file '{fullFilePath}'. Error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // 處理權限不足的錯誤。
                throw new UnauthorizedAccessException($"Error: Access denied for file '{fullFilePath}'. Error: {ex.Message}");
                // 通常遇到權限問題會選擇拋出，但為了讓其他文件盡可能被讀取，這裡也選擇記錄並跳過。
            }
        }

        // 6. 返回結果列表
        return fileContentsList;
    }

    /// <summary>
    /// 將位元組內容儲存到伺服器上的指定目錄，並在儲存前清除該目錄下的所有舊檔案。
    /// </summary>
    /// <param name="directoryPath">檔案儲存的目錄路徑。</param>
    /// <param name="fileName">要儲存的檔案名稱。</param>
    /// <param name="fileBytes">檔案的位元組內容。</param>
    /// <returns>操作成功返回 true，否則返回 false。</returns>
    public static bool SaveFileToServer(string directoryPath, string fileName, byte[] fileBytes)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(directoryPath) || string.IsNullOrWhiteSpace(fileName))
                return false; // 基本檢查失敗

            // 1. 確保目錄存在
            if (!Directory.Exists(directoryPath))
            {
                // 使用 Directory.CreateDirectory 可以處理路徑中不存在的任何部分
                Directory.CreateDirectory(directoryPath);
            }
            else
            {
                // 刪除目錄下所有檔案
                foreach (string filePath in Directory.EnumerateFiles(directoryPath))
                {
                    // 使用 File.Delete 進行刪除
                    File.Delete(filePath);
                }
            }
            // 使用 FileStream 寫入檔案
            // 3. 構建完整檔案路徑
            var fullFilePath = Path.Combine(directoryPath, fileName);

            // 4. 使用 File.WriteAllBytes 進行原子寫入，更簡潔高效
            //    這一步會自動處理 FileMode.Create (覆寫) 和 FileAccess.Write，並自動關閉資源。
            File.WriteAllBytes(fullFilePath, fileBytes);

            return true;
        }
        catch (IOException ioEx)
        {
            // 檔案或目錄操作失敗 (例如：權限不足、檔案被佔用)
            throw new IOException($"I/O Error saving file: {ioEx.Message}");
        }
        catch (UnauthorizedAccessException uaEx)
        {
            // 權限不足
            throw new UnauthorizedAccessException($"Access Denied: {uaEx.Message}");
        }
        catch (Exception ex)
        {
            // 其他未預期的異常
            throw new Exception($"Unexpected Error: {ex.Message}");
        }
    }


    /// <summary>
    /// Clears the contents of a specified directory, with built-in retry logic for locked files.
    /// </summary>
    /// <param name="directoryPath">The path to the directory to clear.</param>
    /// <param name="clearSubDir">If true, recursively deletes all subdirectories and their contents.</param>
    /// <returns>True if the directory content was successfully cleared; otherwise, false.</returns>
    public static bool ClearDirectoryContent(string directoryPath, bool clearSubDir = true)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            return false;

        try
        {
            if (clearSubDir)
            {
                // Delete all subdirectories recursively
                foreach (string dirPath in Directory.EnumerateDirectories(directoryPath))
                {
                    AttemptDeleteDirectory(dirPath);
                }
            }

            // Delete all files in the current directory
            foreach (string filePath in Directory.EnumerateFiles(directoryPath))
            {
                AttemptDeleteFile(filePath);
            }

            return true;
        }
        catch (Exception ex)
        {
            // Re-throw the exception with a clear message indicating the directory was not cleared.
            throw new Exception($"Failed to clear directory content: {ex.Message}", ex);
        }
    }


    public static bool DeleteSpecificFile(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false; // 檔案原本就不存在
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除檔案失敗: {ex.Message}");
        }
    }


    /// <summary>
    /// Attempts to delete a directory, handling sharing violation errors with retry logic.
    /// </summary>
    private static void AttemptDeleteDirectory(string dirPath)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                // true indicates recursive deletion
                Directory.Delete(dirPath, true);
                return; // Success, exit method
            }
            catch (IOException ioEx) when ((ioEx.HResult & 0xFFFF) == SharingViolationHResult)
            {
                // File/directory is locked by another process, log and retry
                if (i < MaxRetries - 1)
                {
                    Thread.Sleep(DelayMilliseconds);
                    continue; // Continue to the next retry attempt
                }
                // If max retries reached, throw a specific exception
                throw new IOException($"Failed to delete locked directory '{dirPath}' after {MaxRetries} attempts.", ioEx);
            }
            catch (Exception)
            {
                // Re-throw other exceptions (Access Denied, etc.)
                throw;
            }
        }
    }

    /// <summary>
    /// Attempts to delete a single file, handling sharing violation errors with retry logic.
    /// </summary>
    private static void AttemptDeleteFile(string filePath)
    {
        for (int i = 0; i < MaxRetries; i++)
        {
            try
            {
                File.Delete(filePath);
                return; // Success, exit method
            }
            catch (IOException ioEx) when ((ioEx.HResult & 0xFFFF) == SharingViolationHResult)
            {
                // File is locked by another process, log and retry
                if (i < MaxRetries - 1)
                {
                    Thread.Sleep(DelayMilliseconds);
                    continue; // Continue to the next retry attempt
                }
                // If max retries reached, throw a specific exception
                throw new IOException($"Failed to delete locked file '{filePath}' after {MaxRetries} attempts.", ioEx);
            }
            catch (Exception)
            {
                // Re-throw other exceptions (Access Denied, etc.)
                throw;
            }
        }
    }

}