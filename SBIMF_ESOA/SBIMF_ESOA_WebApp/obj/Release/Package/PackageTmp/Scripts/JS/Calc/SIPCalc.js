function formatNumber(number) {
    x = number.toString();
    var splitNo = x.split('.');
    var lastThree = splitNo[0].substring(splitNo[0].length - 3);
    var otherNumbers = splitNo[0].substring(0, splitNo[0].length - 3);
    if (otherNumbers != '')
        if (splitNo[1] == undefined) {
            lastThree = ',' + lastThree + '.' + '00';
        }
        else {
            lastThree = ',' + lastThree + '.' + splitNo[1];
        }
    var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree;
    return res
}
function CalculatePmt() {
    var ir = parseFloat($('#ddlexpectedReturn :selected').text());// $(".growth_rate2").slider("value"); //rate Of Interest
    if (!isNaN(ir)) {
        ir /= 100;
    }
    var fv = $("#txtAmount").val().replace(/,/g, "");//$(".amt_invest2").slider("value") * 20000; // Goal Amount
    var np = $("#txtYears").val();//$(".years_invest2").slider("value"); //tenure
    $('.finalresult').html(formatNumber(PMT(ir, np, 0, fv, 12)));
}
function PMT(ir, np, pv, fv, frq) {
    var pmt = 0;
    var target = fv;
    var ROR = ir;
    var tenure = np;
    var noftimes = 12;

    var calcres = (1 + (ROR / noftimes));
    var years = (noftimes * tenure);
    pmt = target / (((Math.pow(calcres, years) - 1) / (ROR / noftimes) * (1 + ROR / noftimes)));
    return (Math.round(pmt));

    $('.tenure').html(formatNumber(np));
    $('.goalAmount').html(formatNumber(fv));
}
function GetFundURL(fundCategoryID, fundName, fundCategoryName) {
    var finalURL, category = "";
    if (parseInt(fundCategoryID) == 8) {
        category = fundCategoryName;
        category = category.replace(/\s+/g, '-');
        category = category.toLowerCase();
    }
    else if (parseInt(fundCategoryID) == 6) { category = "exchange-traded-schemes"; }
    else {
        category = fundCategoryName;
        category = category + "-schemes";
        category = category.toLowerCase();
    }
    var fundNameNew = fundName.replace(/\s[-]\s/g, "-");
    fundNameNew = fundNameNew.replace(/\s\s[-]\s/g, "");
    fundNameNew = fundNameNew.replace(/\s[&]\s /g, "-");
    fundNameNew = fundNameNew.replace(/[']/g, "");
    fundNameNew = fundNameNew.replace(/ /g, "-").toLowerCase();
    return finalURL = "/" + category + "/" + fundNameNew;
}
function GetRcmdFund(url) {
    $.ajax({
        type: "POST",
        dataType: "json",
        async: false,
        crossDomain: true,
        contentType: "application/json;charset=utf-8",
        url: "/_vti_bin/SBIMF/SBIMFService.svc/GetRecmdFund",
        success: function (response) {
            if (response != '') {
                var data = JSON.parse(response);
                var htmlData = '';
                $('.appendFund').html('');
                if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {

                        htmlData += '<li>' + data[i].FundName + '<a class="fr" href="' + url + GetFundURL(data[i].FundCategoryID, data[i].FundName, data[i].CategoryName) + '">APPLY NOW</a></li>'
                    }

                }
                else {
                    htmlData += '<li>No Recommended funds.</li>'
                }
                $('.appendFund').html(htmlData);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Network error please try later')
        }
    });
}
function EncryptEncode(queryStringData, isFolioExist) {
    //var encryptedPAN = "";
    //$.ajax({
    //    type: "POST",
    //    dataType: "json",
    //    url: EncryptEncodeUrl,
    //    data: JSON.stringify({ queryStringData: queryStringData }),
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (resp) {
    //        if (resp != "" && resp.ReturnCode == "0") {
    //            encryptedPAN = resp.Data;
    //            window.open(webAbsoluteUrl + "/investment-solutions?PAN=" + encryptedPAN + "&isFolioExist=" + isFolioExist, '_blank');
    //            $('#kyc-varified-modal').modal('hide');
    //            //window.location = webAbsoluteUrl + "/investment-solutions?PAN=" + encryptedPAN + "&isFolioExist=" + isFolioExist;
    //        }
    //        else {
    //            $("#errorPAN").text('Some error occured while processing request, please try again later.');
    //        }
    //    },
    //    error: function (xhr, status, e) {
    //        $("#errorPAN").text("There seems to be issue with network.");
    //    }
    //});


    var body = JSON.stringify({ queryStringData: queryStringData });

    $.when(commonObj.apiHelper(EncryptEncodeUrl, 'POST', body)).then(function (resp) {
        resp = JSON.parse(resp);
        if (resp != "" && resp.ReturnCode == "0") {
            encryptedPAN = resp.Data;
            window.open(webAbsoluteUrl + "/investment-solutions?PAN=" + encryptedPAN + "&isFolioExist=" + isFolioExist, '_blank');
            $('#kyc-varified-modal').modal('hide');
            //window.location = webAbsoluteUrl + "/investment-solutions?PAN=" + encryptedPAN + "&isFolioExist=" + isFolioExist;
        }
        else {
            $("#errorPAN").text('Some error occured while processing request, please try again later.');
        }
    }, function (errorThrown) {
        $("#errorPAN").text("There seems to be issue with network.");
    }); 
}
function checkKYCAndFolioExist(PAN) {
    var encryptedPAN = "";
    //$.ajax({
    //    type: "POST",
    //    dataType: "json",
    //    url: VerifyPanUrl,
    //    data: JSON.stringify({ PAN: PAN }),
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (resp) {
    //        if (resp != "" && resp != null) {
    //            if (resp.IsKYC != "") {
    //                if (resp.IsKYC == true) {
    //                    $('#kyc-varified-modal').modal('show');
    //                    setTimeout(function () { encryptedPAN = EncryptEncode(PAN, true); }, 3000);
    //                }
    //                else {
    //                    $('#kyc-not-varified-modal').modal('show');
    //                }
    //            }
    //            if (resp.IsKYC == false) {
    //                $('#kyc-varified-modal').modal('hide');
    //                $('#kyc-not-varified-modal').modal('show');
    //            }
    //        }
    //        else {
    //            $("#errorPAN").text('Some error occured while processing request, please try again later.');
    //        }
    //    }
    //    , beforeSend: function () {
    //        //idealake_website_loaderManagement.init();
    //    },
    //    complete: function () {
    //        //idealake_website_loaderManagement.LoaderEnd();
    //    },
    //    error: function (xhr, status, e) {
    //        $("#errorPAN").text("There seems to be issue with network.");
    //    }
    //});


    var body = JSON.stringify({ PAN: PAN });
    $.when(commonObj.apiHelper(VerifyPanUrl, 'POST', body)).then(function (resp) {
        resp = JSON.parse(resp)
        if (resp != "" && resp != null) {
            if (resp.IsKYC != "") {
                if (resp.IsKYC == true) {
                    $('#kyc-varified-modal').modal('show');
                    setTimeout(function () { encryptedPAN = EncryptEncode(PAN, true); }, 3000);
                }
                else {
                    $('#kyc-not-varified-modal').modal('show');
                }
            }
            if (resp.IsKYC == false) {
                $('#kyc-varified-modal').modal('hide');
                $('#kyc-not-varified-modal').modal('show');
            }
        }
        else {
            $("#errorPAN").text('Some error occured while processing request, please try again later.');
        }
    }, function (errorThrown) {
        $("#errorPAN").text("There seems to be issue with network.");
        });

}

$.fn.scrollView = function () {
    return this.each(function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top
        }, 1000);
    });
}

$(document).ready(function () {

    forgeryId = $("#forgeryToken").val();
    // Configure/customize these variables.
    var showChar = 398  // How many characters are shown by default
    var ellipsestext = "...";
    var moretext = "Read More";
    var lesstext = "Read Less";


    $('.more').each(function () {
        var content = $(this).html();

        if (content.length > showChar) {

            var c = content.substr(0, showChar);
            var h = content.substr(showChar, content.length - showChar);

            var html = c + '<span class="moreellipses">' + ellipsestext + '&nbsp;</span><span class="morecontent"><span>' + h + '</span><a href="" class="morelink">' + moretext + '</a></span>';

            $(this).html(html);
        }

    });

    $(".morelink").click(function () {
        if ($(this).hasClass("less")) {
            $(this).removeClass("less");
            $(this).html(moretext);
        } else {
            $(this).addClass("less");
            $(this).html(lesstext);
        }
        $(this).parent().prev().toggle();
        $(this).prev().toggle();
        return false;
    });

    $(".errorPAN").text("");
    $('.calculatorResult').hide();
    $('.btnCalculatePlanner').click(function () {
        var expectedrate = $("#ddlexpectedReturn option:selected").text();
        var category = "Financial Planning Calculators";
        var label = "calculate|" + expectedrate + "";
        var action = "systematic investment planner";
        var events = "calculate systematic investment planner";
        var value = "";
        //EventRecommendationWithoutRedirection(category, label, action, events, value);

        CalculatePmt();
        $("#dvResult").css("display", "block");
        $('#dvResult').scrollView();
        var webUrl = webAbsoluteUrl;
        if ($('.appendFund').length > 0) {
            GetRcmdFund(webUrl);
        }
        $('.calculatorResult').show();
        $('.calculatorMain').hide();
        return false;
    });

    $('.toUpperCase').keyup(function () {
        $(this).val($(this).val().toUpperCase());
    });

    $(".submitPAN").click(function () {
        var isValid = true;
        $(".errorPAN").text("");
        var PAN = $("#txtPAN").val().trim();
        var regex = /^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$/;
        if (PAN == "") {
            $(".errorPAN").text("Please enter PAN");
            isValid = false;
        }
        else if (!regex.test(PAN)) {
            $(".errorPAN").text("Please enter valid PAN");
            isValid = false;
        }
        else {
            var isKYC = checkKYCAndFolioExist(PAN.toUpperCase());
            isValid = true;
        }
    });

});