// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var TMDobject = new Object();
//function loadDoc() {
//    var xhttp = new XMLHttpRequest();
//    var params = 'orem=ipsum&name=binny';
//    xhttp.onreadystatechange = function () {
//        if (this.readyState == 4 && this.status == 200) {
//            document.getElementById("demo").innerHTML = this.responseText;
//            document.getElementById("demo").innerHTML = this.responseXML;
//        }
//    };
//    xhttp.open("GET", "ajax_info.txt", true);
//    xhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
//    xhttp.send(params);
//}

//(function (TMDobject) {
//    TMDobject.SendRequest = function (method, url, headers, body, async) {

//    };
//})(TMDobject);
window.onload = function () {
    //TMDobject.GetMarketHours();
    //alert("hello");
    //var headers = {'content-type':'application/json'};
    //TMDobject.SendAjax('GET', '../api/TDAmeritradeAPI/GetPriceHistory/BAC', headers, '', false, 'text');
};

((TMDobject) => {
    TMDobject.SendAjax = function (method, url, headers, body, async, responseType) {
        //alert(body)
        var xhttp = new XMLHttpRequest();
        var response;
        xhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                if (responseType == 'text') {
                    response = this.responseText;
                }
                else if (responseType == 'xml') {
                    response = this.responseXML;
                }
                //alert(JSON.stringify(response));
            }
            else if (this.readyState == 4 && this.status == 404) {
                $('#infoWindowText').empty();
                $('#infoWindowText').append('<div style=\'width: inherit; height: auto; margin: 0px auto; float: none; text-align: center; font-weight:bold; font-size:19px; border:1px solid #cacaca;\'>' + this.responseText + '</div>');
                TMDobject.openItem('#infoWindow');
            }
        };
        xhttp.open(method, url, async);
        for (var header in headers) {
            //alert(header + ' ' + headers[header]);
            xhttp.setRequestHeader(header, headers[header]);
        }
        //alert('body ' + body);
        xhttp.send(body);
        return response;
    };
    //TMDobject.GetAjax = function (method, url, headers, body, mode, responseType) {
    //    var results;
    //    fetch(url, {
    //        method: method,
    //        //headers: {
    //        //    'Content-Type': 'application/json',
    //        //},
    //        header: headers,
    //        //body: new FormData(document.getElementById("inputform")),
    //        body: body,
    //        // -- or --
    //        // body : JSON.stringify({
    //        // user : document.getElementById('user').value,
    //        // ...
    //        // })
    //        mode: mode, // no-cors, *cors, same-origin
    //    }).then(
    //        //response => response.text() // .json(), etc.
    //        // same as function(response) {return response.text();}
    //        response => {
    //            if (responseType == 'text') {
    //                this.results = response.text();
    //            }
    //            else if (responseType == 'xml') {
    //                this.results = response.json();;
    //            }
    //        }
    //    ).then(
    //        html => { console.log(html); this.results = html }
    //    ).catch((e) => {
    //        //do error
    //    });
    //    return results;
    //};

    TMDobject.openItem = function (item) {
        $(item).fadeIn('slow', function () { });
    };

    TMDobject.closeItem = function (item) {
        $(item).fadeOut('slow', function () { });
    };

    TMDobject.ShowHistory = function (item) {
        var symbol = $.trim(item);
        var previousClose, currentClose, negative, changeCalc, changePercent;
        var headers = { 'content-type': 'application/json' };
        var response = JSON.parse(TMDobject.SendAjax('GET', '../api/TDAmeritradeAPI/GetPriceHistory/' + symbol, headers, '', false, 'text'));
        $("#infoHeader").empty();
        $("#infoHeader").append(CreateInfoHeader($.trim(symbol), false, true));
        $('#infoWindowText').empty();
        $('#infoTitle').empty();
        /*$('#infoWindowText').append('<div style=\'width: inherit; height: auto; margin: 0px auto; float: none; text-align: center; font-weight:bold; font-size:19px; border:1px solid #cacaca;\'>' + symbol.toUpperCase() + '</div>');*/
        $('#infoTitle').append(symbol.toUpperCase());
        //alert(response.candles[0])
        for (var item in response.candles) {
            for (var key in response.candles[item]) {
                //alert(key);
                if (key === 'datetime') {
                    //alert(key);
                    var date = new Date(response.candles[item][key]);
                    $('#infoWindowText').append("<b>" + key + " : </b>" + date.toDateString() + "<br>");
                } else if (key === 'close') {
                    $('#infoWindowText').append("<b>" + key + " : </b>" + response.candles[item][key] + "<br>");
                    if (currentClose != null) {
                        previousClose = currentClose;
                    }
                    currentClose = parseFloat(response.candles[item][key]);
                    if (currentClose != null && previousClose != null) {
                        if (currentClose > previousClose) {
                            negative = "green";
                            changeCalc = (currentClose - previousClose).toFixed(2);
                        }
                        else if (previousClose > currentClose) {
                            negative = "red";
                            changeCalc = (previousClose - currentClose).toFixed(2);
                        }
                        else {
                            negative = "grey";
                            changeCalc = 0;
                        }
                        changePercent = ((changeCalc / previousClose) * 100).toFixed(2);
                        $('#infoWindowText').append("<font color='" + negative + "'><b>Change $: </b>" + changeCalc + "</font><br>");
                        $('#infoWindowText').append("<font color='" + negative + "'><b>Change %: </b>" + changePercent + "</font><br>");
                    }
                }
                else {
                    $('#infoWindowText').append("<b>" + key + " : </b>" + response.candles[item][key] + "<br>");
                }
            }
            $('#infoWindowText').append("<hr>");
        }
        TMDobject.openItem('#infoWindow');
    }

    TMDobject.Search = function (item) {
        var symbol = $.trim(item);
        var headers = { 'content-type': 'application/json' };
        //var response = JSON.parse(TMDobject.SendAjax('GET', '../api/TDAmeritradeAPI/GetQuote/' + symbol, headers, '', false, 'text'));
        var response = TMDobject.SendAjax('GET', '../api/TDAmeritradeAPI/GetQuote/' + symbol, headers, '', false, 'text')
        //var response = TMDobject.SendAjax('GET', '../api/ETradeAPI/Search/' + symbol, headers, '', false, 'text')
        //var response = TMDobject.SendAjax('GET', '../api/ETradeAPI/Quote/' + symbol, headers, '', false, 'text')
        //alert(response);
        if (response == undefined || response == 'undefined') {
            response = { "symbol": symbol, "description": "symbol or company not found" };
        }
        else {
            response = JSON.parse(response);
        }
        $("#infoHeader").empty();
        $("#infoHeader").append(CreateInfoHeader(symbol, true, false));
        $('#infoWindowText').empty();
        $('#infoTitle').empty();
        /*$('#infoWindowText').append('<div style=\'width: inherit; height: auto; margin: 0px auto; float: none; text-align: center; font-weight:bold; font-size:19px; border:1px solid #cacaca;\'>' + symbol.toUpperCase() + '</div>');*/
        $('#infoTitle').append(symbol.toUpperCase());
        //alert(response[symbol.toUpperCase()] + " " + symbol.toUpperCase());
        //alert(response + " " + symbol.toUpperCase());
        //for (var item in response[symbol.toUpperCase()]) {
        for (var item in response) {
            //$('#infoWindowText').append("<b>" + item + " : </b>" + response[symbol.toUpperCase()][item] + "<br>");
            if(item != 'symbol')
                $('#infoWindowText').append("<b>" + item + " : </b>" + response[item] + "<br>");
        }        
        TMDobject.openItem('#infoWindow');
    }
    TMDobject.GetMarketHours = function () {
        var headers = { 'content-type': 'application/json' };
        var response = JSON.parse(TMDobject.SendAjax('GET', '../api/TDAmeritradeAPI/GetMarketHours', headers, '', false, 'text'));
        $('#divMarketHours').empty();
        $('#divMarketHours').append('<h4>Market Hours</h4>');
        $('#divMarketHours').append("<b>Todays Date: </b>" + response.equity.EQ.date + "<br>");
        $('#divMarketHours').append("<b><font color='blue'>Pre Market</font> Start: </b>" + response.equity.EQ.sessionHours.preMarket[0].start + "<b> End: </b>" + response.equity.EQ.sessionHours.preMarket[0].end + "<br>");
        $('#divMarketHours').append("<b><font color='blue'>Regular Market</font> Start: </b>" + response.equity.EQ.sessionHours.regularMarket[0].start + "<b> End: </b>" + response.equity.EQ.sessionHours.regularMarket[0].end + "<br>");
        $('#divMarketHours').append("<b><font color='blue'>Post Market</font> Start: </b>" + response.equity.EQ.sessionHours.postMarket[0].start + "<b> End: </b>" + response.equity.EQ.sessionHours.postMarket[0].end + "<br>");
        //$('#divMarketHours').append(JSON.stringify(response));
    }

    TMDobject.GetAuthorizationToken = function (code) {
        var headers = { 'content-type': 'application/x-www-form-urlencoded' };
        //var response = JSON.parse(TMDobject.SendAjax('Post', '../api/TDAmeritradeAPI/GetAuthorizationToken', headers, "code=" + code, false, 'text'));
        var response = TMDobject.SendAjax('Post', '../api/TDAmeritradeAPI/GetAuthorizationToken', headers, "code=" + code, false, 'text');
        alert('response ' + response);
        //$('#divAccountInfo').empty();
        //$('#divAccountInfo').append('<h4>Account Info</h4>');
        //$('#divAccountInfo').append(JSON.stringify(response));
        //TMDobject.GetAccountInfo(response.access_token);
    }

    TMDobject.GetAccountInfo = function (token) {        
        var headers = { 'content-type': 'application/json' };
        var response = JSON.parse(TMDobject.SendAjax('POST', '../api/TDAmeritradeAPI/GetAccountInfo', headers, token, false, 'text'));
        $('#divAccountInfo').empty();
        $('#divAccountInfo').append('<h4>Account Info</h4>');
        $('#divAccountInfo').append("<b>json: </b> $" + response + "<br>");

    }

    TMDobject.SearchCoinBase = function (product) {
        var headers = { 'content-type': 'application/json', "Accept": "application/json" };
        //var response = JSON.parse(TMDobject.SendAjax('GET', '../api/CoinBaseAPI/GetProduct/' + product + '/ticker', headers, '', false, 'text'));
        var response = JSON.parse(TMDobject.SendAjax('GET', '../api/CoinBaseAPI/GetProduct/' + product + '/stats', headers, '', false, 'text'));
        $('#infoWindowText').empty();
        $('#infoWindowText').append('<div style=\'width: inherit; height: auto; margin: 0px auto; float: none; text-align: center; font-weight:bold; font-size:19px; border:1px solid #cacaca;\'>' + product + '</div>');
        //$('#infoWindowText').append('<h4>' + product + '</h4>');
        var results = JSON.parse(response);
        if (results.message === "NotFound") {
            $('#infoWindowText').append("<b>Product Not Found</b>");
        }
        else {
            //$('#infoWindowText').append(JSON.stringify(response));
            $('#infoWindowText').append("<b>Open: </b> $" + results.open + "<br>");
            $('#infoWindowText').append("<b>High: </b> $" + results.high + "<br>");
            $('#infoWindowText').append("<b>Low: </b> $" + results.low + "<br>");
            $('#infoWindowText').append("<b>Volume: </b> " + results.volume + "<br>");
            $('#infoWindowText').append("<b>Volume 30 Day: </b>" + results.volume_30day + "<br>")
        }
        TMDobject.openItem('#infoWindow');
    }

    function CreateInfoHeader(symbol, showHistory, showSearch) {
        var output = '<button id="btnClose" value="OK" class="btn-sm-blue" onclick="TMDobject.closeItem($(this).parent().parent())">Close</button>&nbsp;';
        output += (showHistory) ? '<button id = "btnClose" value = "OK" class="btn-sm-blue" onclick = "TMDobject.ShowHistory(\'' + symbol + '\')">History</button>&nbsp;' : '';
        output += (showSearch) ? '<button id="btnClose" value="OK" class="btn-sm-blue" onclick="TMDobject.Search(\'' + symbol + '\')">Info</button>&nbsp;' : '';
        return output;
    }

    TMDobject.CheckSubmit = function(e) {
        if (e && e.keyCode == 13) {
            //document.forms[0].submit();
            //alert($('#txtSearch').val());
            if ($.trim($('#txtSearch').val()) != '')
                TMDobject.Search($.trim($('#txtSearch').val()));
        }
    }

})(TMDobject);