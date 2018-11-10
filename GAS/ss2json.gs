function fakeDoGet() {
  var e = { "parameter" : { "sheetName" : "Test" } };
  doGet(e);
}

function doGet(e) {
  var sheetName = e.parameter.sheetName;
  var spreadSheet = SpreadsheetApp.openById("ここは各自で埋める");
  var sheet = spreadSheet.getSheetByName(sheetName);
  if (sheet != null) {
    var json = convertSheetToJson(sheet);
    return ContentService.createTextOutput(JSON.stringify(json)).setMimeType(ContentService.MimeType.JSON);
  } else {
  }
}

function convertSheetToJson(sheet) {
  var columnStartIndex = 1;
  var rowNum = 1;
  var lastRow = sheet.getLastRow();
  
  var firstRange = sheet.getRange(1, 1, 1, sheet.getLastColumn());
  var nameRowValues = firstRange.getValues();
  var nameColumns = nameRowValues[0];
  
  var secondRange = sheet.getRange(2, 1, 1, sheet.getLastColumn());
  var typeRowValues = secondRange.getValues();
  var typeColumns = typeRowValues[0];
  
  var jsonHeader = new Object();
  for (var j = 0; j < nameColumns.length; j++) {
    var typeStr = typeColumns[j].toString();
    if (typeStr == "ignore") {
      continue;
    }
    jsonHeader[nameColumns[j]] = typeColumns[j];
  }
  
  var rowValues = [];
  var lastColumn = sheet.getLastColumn();
  for (var rowIndex = 3; rowIndex <= lastRow; rowIndex++) {
    columnStartIndex = 1;
    var range = sheet.getRange(rowIndex, columnStartIndex, rowNum, lastColumn);
    var values = range.getValues();
    rowValues.push(values[0]);
  }
  
  var jsonElementArray = [];
  for (var i = 0; i < rowValues.length; i++) {
    var line = rowValues[i];
    var json = new Object();
    for (var j = 0; j < nameColumns.length; j++) {
      if (typeColumns[j].toString() == "ignore") {
        continue;
      }
      json[nameColumns[j]] = line[j];
    }
    jsonElementArray.push(json);
  }
  var jsonArray = [];
  var obj = new Object();
  obj["masterHeader"] = jsonHeader;
  obj["masterArray"] = jsonElementArray;
  
  return obj;
}
