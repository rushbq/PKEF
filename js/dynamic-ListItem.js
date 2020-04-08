/*
    [動態欄位]
    1. 使用前確認公用js是否有日期格式化的函式 (e.g.:Date.prototype.Format = function (fmt)...)
    2. 儲存時要使用 Get_Item(...)
    3. 需搭配bootstrap
*/

/* [新增項目]
   myListID = 項目清單編號
   myItemID = 資料來源-編號
   myItemName = 資料來源-名稱
   showID = 是否要顯示編號
   val1 = 單價
   val2 = 庫存
*/
//BBC-訂單品項
function Add_Item(myListID, myItemID, myItemName, showID, showPrice, val1, val2) {
    //取得參數值
    var ObjID = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");     //自動編號
    var ObjValID = $("#" + myItemID).val();     //資料來源 - 編號
    var ObjValName = $("#" + myItemName).val();     //資料來源 - 名稱
    if (ObjValID == "") {
        alert('Field is empty.');
        return;
    }

    //填入Html
    var NewItem = '<li id="li_' + ObjID + '" class="list-group-item">';
    NewItem += ' <table class="table">';
    NewItem += '     <tbody>';
    NewItem += '         <tr>';
    NewItem += '             <td>';
    NewItem += '                 <h4>' + ObjValName + '</h4>';
    NewItem += '             </td>';
    NewItem += '             <td class="text-right" style="width: 15%">';

    //- 單價 -
    if (showPrice) {
        NewItem += '$ <span id="price_' + ObjID + '">' + val1 + '</span>';
    }

    NewItem += '             </td>';
    NewItem += '             <td class="text-center" style="width: 15%">';

    //- 數量 -
    NewItem += '                 <input type="text" id="qty_' + ObjID + '" value="1" class="Item_Qty text-center" style="width: 60px;" maxlength="8" onkeyup="checkNum(this)" />';
    NewItem += '             </td>';
    NewItem += '             <td class="text-right" style="width: 20%">';

    //- 小計 -
    if (showPrice) {
        NewItem += '<strong>$ <span id="subtotal_' + ObjID + '">' + val1 + '</span></strong>';
    }
    NewItem += '             </td>';
    NewItem += '         </tr>';
    NewItem += '         <tr>';
    NewItem += '             <td>';

    //- 品號 -
    if (showID) {
        NewItem += '<label class="label label-warning">' + ObjValID + '</label>';
    }
    NewItem += '             </td>';
    NewItem += '             <td colspan="2" class="text-right">';

    //- 庫存 -
    if (showID) {
        NewItem += '<abbr title="目前庫存">庫存：' + val2 + '</abbr>';
        NewItem += '<input type="hidden" class="Item_Stock" value="' + val2 + '">';
    }
    NewItem += '             <td class="text-right">';
    NewItem += '                 <button type="button" class="btn btn-default btn-xs" onclick="Delete_Item(\'' + ObjID + '\');"><i class="fa fa-times"></i>&nbsp;移除</button>';
    NewItem += '             </td>';
    NewItem += '         </tr>';
    NewItem += '     </tbody>';
    NewItem += ' </table>';


    //隱藏欄位, 儲存時調用(放在li裡, 若要調動需調整Get_Item(..))
    NewItem += '<input type="hidden" class="Item_ID" value="' + ObjValID + '" />';
    NewItem += '<input type="hidden" class="Item_Name" value="' + ObjValName + '" />';

    NewItem += '</li>';
    

    //將項目append到指定控制項
    $("#" + myListID).append(NewItem);
}

//BBC-出貨單-箱子
function Add_Item_ShipBox(myListID, ObjValID, ObjValName) {
    //取得參數值
    var ObjID = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");     //自動編號
    if (ObjValID == "") {
        alert('Field is empty.');
        return;
    }

    //填入Html
    var NewItem = '<li id="li_' + ObjID + '" class="list-group-item">';
    NewItem += ' <table width="100%">';
    NewItem += '         <tr>';
    NewItem += '             <td style="width: 70%">';
    NewItem += '                 <h4>' + ObjValName + '</h4>';
    NewItem += '             </td>';
    //- 數量 -
    NewItem += '             <td class="text-center" style="width: 15%">';
    NewItem += '                 <input type="text" id="qty_' + ObjID + '" value="1" class="Item_Qty text-center" style="width: 60px;" maxlength="8" onkeyup="checkNum(this)" />';
    NewItem += '             </td>';
    NewItem += '             <td class="text-right" style="width: 15%">';
    NewItem += '                 <button type="button" class="btn btn-default btn-xs" onclick="Delete_Item(\'' + ObjID + '\');"><i class="fa fa-times"></i>&nbsp;移除</button>';
    NewItem += '             </td>';
    NewItem += '         </tr>';
    NewItem += ' </table>';


    //隱藏欄位, 儲存時調用(放在li裡, 若要調動需調整Get_Item(..))
    NewItem += '<input type="hidden" class="Item_ID" value="' + ObjValID + '" />';
    NewItem += '<input type="hidden" class="Item_Name" value="' + ObjValName + '" />';

    NewItem += '</li>';

    
    //將項目append到指定控制項
    $("#" + myListID).append(NewItem);
}

//一般關聯
function Add_Item_Normal(myListID, ObjValID, ObjValName, showID) {
    //取得參數值
    var ObjID = new Date().Format("yyyy_MM_dd_hh_mm_ss_S");     //自動編號
    if (ObjValID == "") {
        alert('Field is empty.');
        return;
    }

    //填入Html
    var NewItem = '<li id="li_' + ObjID + '" class="list-group-item">';
    NewItem += ' <table width="100%">';
    NewItem += '         <tr>';
    NewItem += '             <td style="width: 85%">';
    if (showID) { NewItem += '<label class="label label-warning">' + ObjValID + '</label>'; }
    NewItem += '                 <h4>' + ObjValName + '</h4>';
    NewItem += '             </td>';
    NewItem += '             <td class="text-right" style="width: 15%">';
    NewItem += '                 <button type="button" class="btn btn-default btn-xs" onclick="Delete_Item(\'' + ObjID + '\');"><i class="fa fa-times"></i>&nbsp;移除</button>';
    NewItem += '             </td>';
    NewItem += '         </tr>';
    NewItem += ' </table>';


    //隱藏欄位, 儲存時調用(放在li裡, 若要調動需調整Get_Item(..))
    NewItem += '<input type="hidden" class="Item_ID" value="' + ObjValID + '" />'; 
    NewItem += '<input type="hidden" class="Item_Name" value="' + ObjValName + '" />';

    NewItem += '</li>';


    //將項目append到指定控制項
    $("#" + myListID).append(NewItem);
}



/* 刪除指定項目 */
function Delete_Item(myItemID) {
    $("#li_" + myItemID).remove();
}

/* 刪除所有項目 */
function Delete_AllItem(myListID) {
    $("#" + myListID + " li").each(
       function (i, elm) {
           $(elm).remove();
       });
}

/* [取得各項目欄位值]
   myListID = 項目清單編號
   myValField = 存放項目參數值集合的欄位
   myClassID = 欄位Class命名 (ex:item_ID)
*/
function Get_Item(myListID, myValField, myClassID) {
    //取得存放資料的控制項, <欄位X>
    var myFldItem = $("#" + myValField);

    //清空此欄位
    myFldItem.val('');

    //取得項目清單值, 組合後填入<欄位X>, 以逗號分隔
    $("#" + myListID + " li ." + myClassID).each(
        function (index, element) {
            var OldCont = myFldItem.val();
            if (OldCont == '') {
                myFldItem.val($(element).val());
            } else {
                myFldItem.val(OldCont + ',' + $(element).val());
            }
        }
    );
}