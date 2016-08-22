using System.Collections.Generic;
using System.IO;
using System;

#if UNITY_EDITOR

using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

#endif

public class ExcelManager
{
	private const string COPY_NAME_SYMBOL = "CopyFile_";
    
    private static ExcelManager _instance;  
    public static ExcelManager Instance
    {
		get
		{
			if(_instance == null)  
			{  
				_instance = new ExcelManager();  
			}  
			return _instance;  
		}
    }
    
	#region UI Tools
    /*
	public void WriteExcel(string filePath,string sheetName, List<string> listInfo)  
    { 
		#if UNITY_EDITOR
		if(File.Exists(filePath)) File.Delete(filePath);
		
        XlsDocument xls = new XlsDocument();
		xls.FileName = filePath;

        xls.SummaryInformation.Author = "xyy"; 
        xls.SummaryInformation.Subject = "test";
  
        Worksheet sheet = xls.Workbook.Worksheets.AddNamed(sheetName);
        Cells cells = sheet.Cells;
   
		string[] tempArray = null;
		int rowNum = listInfo.Count;  
		int length = 0;
		int k = 0;

        for (int index = 0; index < rowNum; index++)  
        {  
			tempArray = listInfo[index].Split(SPILT_CHAR);

			length = tempArray.Length;
			for(k = 0;k < length;k ++)
			{
				cells.Add(index + 1, k + 1, tempArray[k]);  
			}
        }  

        xls.Save();  
//
//		string destPath = filePath.Replace(".xls",".xlsx");
//		File.Copy (filePath, destPath, true);
//		File.Replace(filePath,destPath,null);
#endif
    }  */

	public DataTable ReadExcel(string filePath,string sheetName)
	{
        return ReadExcelByNpoiDLL(filePath,sheetName);
	}

	private DataTable ReadExcelByNpoiDLL(string filePath,string sheetName)
	{
		#if UNITY_EDITOR
		List<string> list = new List<string> ();

		FileStream fs = null;
		string cellValue = null;
		int cellCount = 0;
		int i = 0;
		int m = 0;
		ICell cell = null;

        string copyFilePath = "";
		try 
		{
			fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		}
		catch(Exception e)
		{
			string fileName = Path.GetFileName(filePath);
			copyFilePath =filePath.Replace(fileName, COPY_NAME_SYMBOL + fileName);
			File.Copy(filePath,copyFilePath,true);
			fs = File.Open(copyFilePath, FileMode.Open, FileAccess.Read);
		}
		
		ISheet sheet = null;
        IWorkbook workbook = null;
        if (filePath.Contains("xlsx"))
            workbook = new XSSFWorkbook(fs);
        else
            workbook = new HSSFWorkbook(fs);

        if (!string.IsNullOrEmpty(sheetName))
		{
			sheet = workbook.GetSheet(sheetName);
			if (sheet == null) 
			{
				UnityEngine.Debug.LogError("Not Find '" + sheetName + "' in the file : " + filePath);
				Close (ref fs, copyFilePath);
				return null;
			}
		}
		else sheet = workbook.GetSheetAt(0); // get first sheet
		
		if (sheet == null)
		{
			UnityEngine.Debug.LogError("No Any Sheets in the file : " + filePath);
			Close (ref fs, copyFilePath);
			return null;
		}

        DataTable data = null;
        data = new DataTable ();
		data.TableName = sheet.SheetName;

        IRow firstRow = null;
        firstRow = sheet.GetRow(0);
		cellCount = firstRow.LastCellNum;
		if(cellCount == 0)
		{
			Close (ref fs, copyFilePath);
			return null;
		}
		
		i = 0;
		do
		{
			cell = firstRow.GetCell(i);
			
			if (cell == null) cellValue = "";
			else cellValue = cell.StringCellValue;
			
			cellValue += "_" + i;
			
			DataColumn column = new DataColumn(cellValue);
			data.Columns.Add(column);
			
			i ++;
		}
		while(i < cellCount);
		
		int rowCount = sheet.LastRowNum;
		if(rowCount == 0)
		{
			Close (ref fs, copyFilePath);
			return null;
		}

        IRow row = null;
        DataRow dataRow = null;
        i = sheet.FirstRowNum;
		do
		{
			row = sheet.GetRow(i);
			if(row == null)
			{
				i ++;
				continue;
			}
			dataRow = data.NewRow();
			
			m = row.FirstCellNum;
			do
			{
				dataRow[m] = row.GetCell(m) != null ? row.GetCell(m).ToString() : "";
				
				m ++;
			}
			while(m < cellCount);
			
			data.Rows.Add(dataRow);
			
			i ++;
		}
		while(i <= rowCount);
		
		Close (ref fs, copyFilePath);        

		return data;
#else
		return null;
#endif
	}

	public void Close(ref FileStream fs, string copyFilePath)
	{
		fs.Close ();
		fs.Dispose ();
		fs = null;

        if(!string.IsNullOrEmpty(copyFilePath))
		{
			File.Delete(copyFilePath);
			copyFilePath = null;
		}
	}
	
	#endregion
}  