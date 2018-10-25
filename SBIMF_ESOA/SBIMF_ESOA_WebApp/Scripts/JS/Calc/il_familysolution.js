var familyUrl = "/_vti_bin/SBIMF/SBIMFTrnsService.svc/";
var serviceUrl = "/_vti_bin/SBIMF/SBIMFTrnsService.svc/";
var SBIMFserviceUrl = "/_vti_bin/SBIMF/SBIMFService.svc/";

var todayDate;
var answers = {};
var methodInput;
var temp;
var equity;
var shortTerm;
var gold;
var plannedYear;
var longTerm;
var monthlySip;
var finalAmount;
var arrSchCode = [];
var arrSchValue = []
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

function GetSchemeValue(schemeCode, categoryType) {
    var value = 0;
    var data = { "Folio_No": " ", "SchemeCode": schemeCode, "TransactionType": "S", "TransactionId": "115" };
    //alert(JSON.stringify(data));
    $.ajax({
        type: "POST",
        async: false,
        url: SBIMFserviceUrl + 'GetSchemeDetails',
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        success: function (resp) {
            var jsonData = JSON.parse(resp);
            todayDate = jsonData[0].ServerDate;
            if (i == 0) {
                value = equity;
                if (value == 0 || value == null) {
                    $("#sch" + i).hide();
                    equity = 0;
                }
                else if (Number(jsonData[0].SIPMinPurchaseAmount) > value) {
                    value = jsonData[0].SIPMinPurchaseAmount;
                    equity = value;
                }
            }
            else if (i == 1) {
                value = shortTerm;

                if (value == 0 || value == null) {
                    $("#sch" + i).hide();
                    shortTerm = 0;
                }
                else if (Number(jsonData[0].SIPMinPurchaseAmount) > value) {
                    value = jsonData[0].SIPMinPurchaseAmount;
                    shortTerm = value
                }
            }
            else if (i == 2) {
                value = longTerm;

                if (value == 0 || value == null) {
                    $("#sch" + i).hide();
                    longTerm = 0;
                }
                else if (Number(jsonData[0].SIPMinPurchaseAmount) > value) {
                    value = jsonData[0].SIPMinPurchaseAmount;
                    longTerm = value;
                    ///////  displayVal += i + ",";
                }
            }
            else if (i == 3) {
                value = gold;

                if (value == 0 || value == null) {
                    $("#sch" + i).hide();
                    gold = 0;
                }
                else if (Number(jsonData[0].SIPMinPurchaseAmount) > value) {
                    value = jsonData[0].SIPMinPurchaseAmount;
                    gold = value;
                }
            }
        },
        error: function (xhr, status, e) {
            alert("There seems to be issue with network.");
        }
    });
    return value;
}
var arrGoalid = '';
function GetParameterValues() {
    var url = window.location.href;

    if (url.split('?')[1] != undefined) {
        var qs = url.split('?')[1];
        var parts = qs.split('&');
        $.each(parts, function () {
            var val = this.split('=')[1];
            arrGoalid = val
        });

    }
}
function GetAllParameterValues() {
    var arrData = null;
    var url = window.location.href;
    if (url.split('?')[1] != undefined) {
        var qs = url.split('?')[1];
        var parts = qs.split('&');
        $.each(parts, function () {
            var val = this.split('=')[1]
            arrData = val
        });

    }
    return arrData;
}
function pupulateSchemeTable(json, i) {
    var nav = parseFloat(json.NAV);
    var schemevalue = GetSchemeValue(json.SchemeCode, i);
    // used for calculating final monthly investment amount and setting table when urn generated after sip
    if (i == 0) {
        var schemeAmt0 = schemevalue
        $('#invstAmt1').append(schemeAmt0)
        $('#fundName1').append(json.SchemeName);
    }
    else if (i == 1) {
        var schemeAmt1 = schemevalue
        $('#invstAmt2').append(schemeAmt1)
        $('#fundName2').append(json.SchemeName);
    }
    else if (i == 2) {
        var schemeAmt2 = schemevalue
        $('#invstAmt3').append(schemeAmt2)
        $('#fundName3').append(json.SchemeName);
    }
    else if (i == 3) {
        var schemeAmt3 = schemevalue
        $('#invstAmt4').append(schemeAmt3)
        $('#fundName4').append(json.SchemeName);
    }
    //finalAmount = schemeAmt0 + schemeAmt1 + schemeAmt2 + schemeAmt3
    var htmlData = '';
    htmlData += '                <tr>                                                                                                                                                                                                                                                                                                                '
    htmlData += '                    <td data-schemeCode="' + json.SchemeCode + '" id="sch' + i + '">                                                                                                                                                                                                                                                                                                            '
    htmlData += '                        <span id="scheme' + i + '">' + json.SchemeName + '</span>                                                                                                                                                                                                                                                              '
    htmlData += '                    </td>                                                                                                                                                                                                                                                                                                           '
    htmlData += '                    <td>                                                                                                                                                                                                                                                                                                            '
    htmlData += '                        <span data-Amount="' + schemevalue + '" id="schemeVal' + i + '">' + schemevalue + '</span>                                                                                                                                                                                                                                                                '
    htmlData += '                    </td>                                                                                                                                                                                                                                                                                                           '
    htmlData += '                    <td>                                                                                                                                                                                                                                                                                                            '
    htmlData += '                        <span id="lblPosition">' + nav.toFixed(2) + '</span>                                                                                                                                                                                                                                                                       '
    htmlData += '                    </td>                                                                                                                                                                                                                                                                                                           '
    htmlData += '                </tr>                                                                                                                                                                                                                                                                                                               '
    htmlData += '                                                                                                                                                                                                                                                                                                                                    '
    return htmlData;
}
function PopulateScheme(json, i) {
    var schemeDetails = '';

    var nav = parseFloat(json.NAV);
    if (json.SchemeCode != '') {
        var schemevalue = GetSchemeValue(json.SchemeCode, i);
    }
    schemeDetails += '              <div class="col-lg-6 col-md-6 col-sm-12 col-xs-12 mrB20">                                                                                         ';
    schemeDetails += '            <div class="choose-goal-box border-gray clearfix">                                                                                          ';
    schemeDetails += '         <div class="recom-fund" data-schemeCode=' + json.SchemeCode + ' id="sch' + i + '">                                                   ';
    schemeDetails += '             <span id="scheme' + i + '"><h4 class="head4-2">' + json.SchemeName + '</h4></span>                        ';
    schemeDetails += '            Monthly SIP - <span class="openB gry" data-Amount=' + schemevalue + ' id="schemeVal' + i + '"><img class="img-mid" alt="₹" src="/_catalogs/masterpage/images/rupee-black-small.png">' + schemevalue + '</span>          ';
    schemeDetails += '         </div>                                                                     ';
    schemeDetails += '         <div class="latest-nav">                                                   ';
    schemeDetails += '             <p>Latest NAV </p>                                                  ';
    schemeDetails += '             <div><img class="img-scheme" src="/_catalogs/masterpage/images/rupee-white1.png" alt="₹">' + nav.toFixed(2) + '</div>                                                     ';
    schemeDetails += '         </div>                                                                     ';
    schemeDetails += '         </div>                                                                     ';
    schemeDetails += '         </div>                                                                     ';

    return schemeDetails;
}
function PopulateSchemes(json, i) {
    var schemeDetails = '';

    var nav = parseFloat(json.NAV);
    if (json.SchemeCode != '') {
        var schemevalue = GetSchemeValue(json.SchemeCode, i);
    }
    schemeDetails += '<div class="fund-latest-nav recom-fund" data-schemeCode=' + json.SchemeCode + ' id="sch' + i + '">';
    schemeDetails += '<div class="strip-latest-nav open-sanssemibold">Latest NAV <i class="fa fa-inr"></i> ' + nav.toFixed(2) + '</div>';
    schemeDetails += '<h4 class="open-sanssemibold f-size18 color4f">' + json.SchemeName + '</h4>';
    schemeDetails += '<p class="f-size16 color89 open-sansregular">Monthly SIP - <i class="fa fa-inr"></i><span  id="schemeVal' + i + '"> ' + schemevalue + '</span></p>';
    schemeDetails += '</div>';

    return schemeDetails;
}


function PopulateData(json) {
    //Json for 
    var dataAppend = '';
    for (i = 0; i < json.length; i++) {
        dataAppend += '                     <div class="questn-box clearfix">                                                                                      ';
        dataAppend += '                    <div class="questn-no"><span>' + [i + 1] + '</span></div>                                                               ';
        dataAppend += '                    <div class="questn-right" data-questionid=' + json[i].QuestionID + '>                                                   ';
        dataAppend += '                        <div class="questn">' + json[i].Question + '</div>                                                                  ';
        dataAppend += '                        <div class="optn-row">                                                                                              ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" class="rdoA" checked=checked name="options' + i + '" value="a"></span>        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionA + '</span>                                                      ';
        dataAppend += '                            </span>                                                                                                         ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" name="options' + i + '" value="b"></span>                        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionB + '</span>                                                      ';
        dataAppend += '                            </span>                                                                                                         ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" name="options' + i + '" value="c"></span>                        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionC + ' </span>                                                     ';
        dataAppend += '                 </span>                                                                                                                    ';
        dataAppend += '                 <span class="optn">                                                                                                        ';
        dataAppend += '                     <span class="radio-bx"><input type="radio" name="options' + i + '" value="d"></span>                                   ';
        dataAppend += '                     <span class="optn-text">' + json[i].OptionD + '</span>                                                                 ';
        dataAppend += '                 </span>                                                                                                                    ';
        dataAppend += '             </div>                                                                                                                         ';
        dataAppend += '         </div>                                                                                                                             ';
        dataAppend += '     </div>                                                                                                                                 ';
    }
    return dataAppend;
}

function PopulateQuestionData(json) {
    //Json for 
    var dataAppend = '';
    for (i = 0; i < json.length; i++) {
        dataAppend += '                     <div class="questn-box clearfix">                                                                                      ';
        dataAppend += '                    <div class="questn-no"><span>' + [i + 1] + '</span></div>                                                               ';
        dataAppend += '                    <div class="questn-right" data-questionid=' + json[i].QuestionID + '>                                                   ';
        dataAppend += '                        <div class="questn">' + json[i].Question + '</div>                                                                  ';
        dataAppend += '                        <div class="optn-row">                                                                                              ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" class="rdoA" checked=checked name="options' + i + '" value="a"></span>        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionA + '</span>                                                      ';
        dataAppend += '                            </span>                                                                                                         ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" name="options' + i + '" value="b"></span>                        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionB + '</span>                                                      ';
        dataAppend += '                            </span>                                                                                                         ';
        dataAppend += '                            <span class="optn">                                                                                             ';
        dataAppend += '                                <span class="radio-bx"><input type="radio" name="options' + i + '" value="c"></span>                        ';
        dataAppend += '                                <span class="optn-text">' + json[i].OptionC + ' </span>                                                     ';
        dataAppend += '                 </span>                                                                                                                    ';
        dataAppend += '                 <span class="optn">                                                                                                        ';
        dataAppend += '                     <span class="radio-bx"><input type="radio" name="options' + i + '" value="d"></span>                                   ';
        dataAppend += '                     <span class="optn-text">' + json[i].OptionD + '</span>                                                                 ';
        dataAppend += '                 </span>                                                                                                                    ';
        dataAppend += '             </div>                                                                                                                         ';
        dataAppend += '         </div>                                                                                                                             ';
        dataAppend += '     </div>                                                                                                                                 ';
    }
    return dataAppend;
}



function AppendRiskQuestions(finalData) {
    $('.questionnairecontainer').html('');
    $('.questionnairecontainer').append(finalData);

}

function LoadQuestions() {

    $.ajax({
        type: "POST",
        async: false,
        url: familyUrl + 'GetQuestionsandOptions',
        contentType: 'application/json; charset=utf-8',
        success: function (resp) {
            //alert("Success Resp : " + JSON.stringify(resp));
            json = JSON.parse(resp);
            if (json.length > 0) {
                var finalData = PopulateData(json);
                AppendRiskQuestions(finalData);
            }
            else {
            }

        },
        beforeSend: function () {
            idealake_website_loaderManagement.init();
            // Loader();
        },
        complete: function () {
            // Code to hide spinner.
            idealake_website_loaderManagement.LoaderEnd();
        },
        error: function (xhr, status, e) {
            //  $(".loading").hide();
            alert("There seems to be issue with network.");
        }
    });


}

var sliderToolTipAmount = $('<span id="sliderToolTipAmount" class="sliderTooltip"></span>');
var sliderToolTipDuration = $('<span id="sliderToolTipDuration" class="sliderTooltip"></span>');
var sliderToolTipInflation = $('<span id="sliderToolTipInflation" class="sliderTooltip"></span>');
var sliderToolTipLumpsum = $('<span id="sliderToolTipLumpsum" class="sliderTooltip"></span>');

function GetCalculatorPageURL(goalID) {
    var URL = '';
    switch (goalID) {
        case '1':
            // URL = "/en-us/financial-planning-calculators/family-solution-tool/retirement-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/retirement-goal";
            break;
        case '2':
            // URL = "/en-us/financial-planning-calculators/family-solution-tool/dream-house-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/dream-house-goal";
            break;
        case '3':
            // URL = "/en-us/financial-planning-calculators/family-solution-tool/children-education-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/children-education-goal";
            break;
        case '4':
            // URL = "/en-us/financial-planning-calculators/family-solution-tool/childs-wedding-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/childs-wedding-goal";
            break;
        case '5':
            //URL = "/en-us/financial-planning-calculators/family-solution-tool/vacation-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/vacation-goal";

            break;
        case '6':
            // URL = "/en-us/financial-planning-calculators/family-solution-tool/apna-goal?GoalID=" + goalID;
            URL = "/en-us/financial-planning-calculators/family-solution-tool/apna-goal";
            break;


    }

    return URL;
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
function Validate() {
    var isValid = true;
    $('.erroryears').html('');
    $('.erroramt').html('');
    $('.errorinflation').html('');
    $('.lumpsumerrors').html('');
    var age = $("#txtYears").val();
    var amount = $("#txtAmount").val();
    var inflation = $("#txtInflation").val();
    var lumpsum = $("#txtlumpsum").val();
    if (age == '') {
        $('.erroryears').html('Please enter age.');
        isValid = false;
    }
    else if (age < 1 || age > 30) {
        $('.erroryears').html('Plan must be for maximum 30 years.');
        isValid = false;
    }
    if (amount == '') {
        $('.erroramt').html('Please enter amount.');
        isValid = false;
    }
    else if (!isNaN(amount)) {
        if (amount < 10000) {
            $('.erroramt').html('Amount must be greater than zero and minimum 10000.');
            isValid = false;
        }
    }
    if (inflation == '') {
        $('.errorinflation').html('Please enter inflation.');
        isValid = false;
    }
    else if (inflation < 1 || inflation > 15) {
        $('.errorinflation').html('Invalid');
        isValid = false;
    }

    if (lumpsum == '') {
        $('.lumpsumerrors').html('Please enter lumpsum.');
        isValid = false;
    }

    return isValid;
}


//function SendAnswers(methodInput) {
//    var Details = {};
//    Details.strAnswers = JSON.stringify(methodInput);
//    $.ajax({
//        type: "POST",
//        async: false,
//        url: familyUrl + 'RiskQuestionsAnswered',
//        data: JSON.stringify(Details),
//        dataType: 'json',
//        contentType: 'application/json; charset=utf-8',
//        success: function (resp) {
//            var jsonData = JSON.parse(resp);
//            if (jsonData) {
//                //Adding the Risk assestment score in cookie
//                //idealake_cpManager.AddCookie(jsonData, 'il_riskAssestmentScore');
//                // TODO jsonData risk id
//                var userId = SessionResponse.UserId;
//                if (userId == null) {
//                    SaveRiskAssessment(jsonData, '')
//                }
//                else {
//                    SaveRiskAssessment(jsonData, userId)
//                }
//                //SaveRiskAssessment(jsonData, userId)
//                var RedirectUrl = GetCalculatorPageURL(idealake_cpManager.GetCookie('il_homePageGoalID'));
//                //Get Redirect URL

//                // alert(idealake_cpManager.GetCookie('il_riskAssestmentScore'))
//                if (RedirectUrl != '') {
//                    window.location = RedirectUrl;
//                }
//                else {
//                    window.location = '/en-us/financial-planning-calculators/family-solution-tool';
//                }
//            }
//        },
//        beforeSend: function () {
//            idealake_website_loaderManagement.init();
//            // Loader();
//        },
//        complete: function () {
//            // Code to hide spinner.
//            idealake_website_loaderManagement.LoaderEnd();
//        },
//        error: function (xhr, status, e) {
//            //$(".loading").hide();
//            alert("There seems to be network issue..");
//        }
//    });

//}

function SaveRiskAssessment(riskAssessmentId, userId) {
    var parm = [];
    parm[0] = "RiskProfileID=" + riskAssessmentId;
    parm[1] = "UserId=" + userId
    var parameters = { "parameters": parm }
    $.ajax({
        type: "POST",
        data: JSON.stringify(parameters),
        dataType: "json",
        async: false,
        crossDomain: true,
        contentType: "application/json;charset=utf-8",
        url: "/_vti_bin/SBIMF/SBIMFService.svc/SaveRiskAssessmentData",
        success: function (response) {
            //if (response != '') {
            //    var jsonData = JSON.parse(response);
            //}
        }
    });
}

function GetInflation(data) {
    var pData = { "goalid": data };
    $.ajax({
        type: "POST",
        async: false,
        url: serviceUrl + 'GetInflationValueByGoalId',
        data: JSON.stringify(pData),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resp) {

            jsonData = JSON.parse(resp);

            inflaVal = parseFloat(jsonData[0].InflationValue);
            var txtvalue = Math.round(inflaVal * 100) / 100
            $("#txtInflation").val(txtvalue);
            $("#sliderInflation").exponentialslider("finalvalue", txtvalue);
            var InitialInflationValue = txtvalue + '%';
            sliderToolTipInflation.html(InitialInflationValue);
        },
        beforeSend: function () {
            idealake_website_loaderManagement.init();
            // Loader();
        },
        complete: function () {
            // Code to hide spinner.
            idealake_website_loaderManagement.LoaderEnd();
        },
        error: function (xhr, status, e) {
            //$('.loading').hide();
            alert("There seems to be issue with network.");
        }
    });
}
//Format number in comma seperated(Indain Format ex 10,00,000)
function CommaSeperatedNumber(val) {
    var value = val.toString();
    var Numbers = value.split(".");
    var NumberWithoutDecimal = Numbers[0];
    var lastThree = NumberWithoutDecimal.substring(NumberWithoutDecimal.length - 3);
    var otherNumbers = NumberWithoutDecimal.substring(0, NumberWithoutDecimal.length - 3);
    if (otherNumbers != '')
        lastThree = ',' + lastThree;
    Numbers[0] = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree;
    var res = Numbers.join(".");
    return (res);
}

//Append string Cr,Lac,K 
function numDifferentiation(val) {
    if (val >= 10000000) {
        val = (val / 10000000);
        if (val.toString().indexOf('.') != -1) { val = parseFloat(val).toFixed(2); }
        val = val + ' Cr';
    }

    else if (val >= 100000) {
        val = (val / 100000);
        if (val.toString().indexOf('.') != -1) { val = parseFloat(val).toFixed(2); }
        val = val + ' Lac';
    }
    else if (val >= 1000) {
        val = (val / 1000);
        if (val.toString().indexOf('.') != -1) { val = parseFloat(val).toFixed(2); }
        val = val + ' K';
    }
    return val;
}

function Sliders() {
    sliderToolTipDuration.html('');
    sliderToolTipAmount.html('');
    sliderToolTipInflation.html('');
    sliderToolTipLumpsum.html('');

    $("#sliderYears").exponentialslider({
        logicalmin: 1,
        logicalmax: 30,
        logicalstep: 1,
        slidertype: 'linear',
        exponentialslide: onSlider2Move1

    });
    $('#sliderYears').find('.ui-slider-handle').append(sliderToolTipDuration);
    var InitialDurationValue = '1 Yr';
    sliderToolTipDuration.html(InitialDurationValue);

    function onSlider2Move1(e, ui) {
        var x = ui.finalvalue;
        $('#txtYears').val(x);
        var DurationValue = x + ' Yrs';
        if (x == 1) DurationValue = x + ' Yr';
        sliderToolTipDuration.html(DurationValue);
        $(sliderToolTipDuration).removeClass('HideToolTip');
        $('.erroryears').html('');

        if (x < 3) { $(".sliderYearsMin").css("display", "none"); }
        else { $(".sliderYearsMin").css("display", "block"); }
        if (x > 25) { $(".sliderYearsMax").css("display", "none"); }
        else { $(".sliderYearsMax").css("display", "block"); }
    }
    $('#txtYears').change(function () {
        var data = this.value;
        if (data != "") {
            $(sliderToolTipDuration).removeClass('HideToolTip');
            $("#sliderYears").exponentialslider("finalvalue", data);
            var DurationValue = data + ' Yrs';
            if (data == 1) DurationValue = data + ' Yr';
            sliderToolTipDuration.html(DurationValue);
            $('.erroryears').html('');
        }
        else {
            $("#sliderYears").exponentialslider("finalvalue", "0");
            sliderToolTipDuration.html("");
            $(sliderToolTipDuration).addClass('HideToolTip');
        }
    })
    //  });

    //slider For Amount
    //$(function () {
    $("#sliderAmount").exponentialslider({
        logicalmin: 10000,
        logicalmax: 1000000000,
        logicalstep: 10000,
        slidertype: 'exponential',
        exponentialslide: onSlider2Move2
    });
    $('#sliderAmount').find('.ui-slider-handle').append(sliderToolTipAmount);
    var InitialValue = 10000;
    var DisplayInitailValue = numDifferentiation(InitialValue);
    sliderToolTipAmount.html(DisplayInitailValue);

    $('#txtAmount').change(function () {
        var data = this.value;
        if (data != "") {
            var numberWithoutComma = data.toString().replace(/,/g, "");
            data = parseFloat(numberWithoutComma);
            $("#sliderAmount").exponentialslider("finalvalue", data);
            var DisplayInitailValue = numDifferentiation(data);
            sliderToolTipAmount.html(DisplayInitailValue);
            $(this).val(CommaSeperatedNumber(data));
            $('.erroramt').html('');
            $(sliderToolTipAmount).removeClass('HideToolTip');
        }
        else {
            $("#sliderAmount").exponentialslider("finalvalue", "0");
            sliderToolTipAmount.html("");
            $(sliderToolTipAmount).addClass('HideToolTip');
        }
    })
    function onSlider2Move2(e, ui) {
        var x = ui.finalvalue;
        if (x == 1000000000) {
            x -= 10000000;
        }
        var formattedValue = CommaSeperatedNumber(x);
        $('#txtAmount').val(formattedValue);
        var SliderValue = x;
        var DisplayedValue = numDifferentiation(SliderValue);
        sliderToolTipAmount.html(DisplayedValue);
        $('.erroramt').html('');
        $(sliderToolTipAmount).removeClass('HideToolTip');

        if (x < 40000) { $(".sliderAmountMin").css("display", "none"); }
        else { $(".sliderAmountMin").css("display", "block"); }
        if (x > 200000000) { $(".sliderAmountMax").css("display", "none"); }
        else { $(".sliderAmountMax").css("display", "block"); }

    }

    /**************RangeSlider for lumpsum*******************/

    //slider for Lumpsum
    //$(function () {
    $("#sliderLumpsum").exponentialslider({
        logicalmin: 100,
        logicalmax: 1000000000,
        logicalstep: 100,
        slidertype: 'exponential',
        exponentialslide: onSlider2Move3
    });
    $('#sliderLumpsum').find('.ui-slider-handle').append(sliderToolTipLumpsum);
    var LumpsumInitialValue = 0;
    var DisplayInitailLumpsumValue = numDifferentiation(LumpsumInitialValue);
    sliderToolTipLumpsum.html(DisplayInitailLumpsumValue);

    function onSlider2Move3(e, ui) {
        var x = ui.finalvalue;
        if (x == 1000000000) {
            x -= 10000000;
        }
        var formattedValue = CommaSeperatedNumber(x);
        $('#txtlumpsum').val(formattedValue);
        var LumpsumSliderValue = x;
        var LumpsumDisplayedValue = numDifferentiation(LumpsumSliderValue);
        sliderToolTipLumpsum.html(LumpsumDisplayedValue);
        $('.lumpsumerrors').html('');
        $(sliderToolTipLumpsum).removeClass('HideToolTip');

        if (x < 500) { $(".sliderLumpsumMin").css("display", "none"); }
        else { $(".sliderLumpsumMin").css("display", "block"); }
        if (x > 200000000) { $(".sliderLumpsumMax").css("display", "none"); }
        else { $(".sliderLumpsumMax").css("display", "block"); }

    }
    $('#txtlumpsum').change(function () {
        var data = this.value;
        if (data != "") {
            var numberWithoutComma = data.toString().replace(/,/g, "");
            data = parseFloat(numberWithoutComma);
            $("#sliderLumpsum").exponentialslider("finalvalue", data);
            var LumpsumDisplayValue = numDifferentiation(data);
            sliderToolTipLumpsum.html(LumpsumDisplayValue);
            $(this).val(CommaSeperatedNumber(data));
            $('.lumpsumerrors').html('');
            $(sliderToolTipLumpsum).removeClass('HideToolTip');
        }
        else {
            $("#sliderLumpsum").exponentialslider("finalvalue", "0");
            sliderToolTipLumpsum.html("");
            $(sliderToolTipLumpsum).addClass('HideToolTip');
        }
    })
    //});
    //slider For Inflation
    //$(function () {
    $("#sliderInflation").exponentialslider({
        logicalmin: 1,
        logicalmax: 15,
        logicalstep: 1,
        slidertype: 'linear',
        exponentialslide: onSlider2Move4
    });
    $('#sliderInflation').find('.ui-slider-handle').append(sliderToolTipInflation);
    var InitialInflationValue = $('#txtInflation').val() + '%';
    sliderToolTipInflation.html(InitialInflationValue);

    function onSlider2Move4(e, ui) {
        var x = ui.finalvalue;
        $('#txtInflation').val(x);
        var InflationValue = x + ' %';
        sliderToolTipInflation.html(InflationValue);
        $('.errorinflation').html('');
        $(sliderToolTipInflation).removeClass('HideToolTip');

        if (x < 2) { $(".sliderInflationMin").css("display", "none"); }
        else { $(".sliderInflationMin").css("display", "block"); }
        if (x > 13) { $(".sliderInflationMax").css("display", "none"); }
        else { $(".sliderInflationMax").css("display", "block"); }

    }

    $('#txtInflation').change(function () {
        var data = this.value;
        if (data != "") {
            $("#sliderInflation").exponentialslider("finalvalue", data);
            var InflationValue = data + ' %';
            sliderToolTipInflation.html(InflationValue);
            $('.errorinflation').html('');
            $(sliderToolTipInflation).removeClass('HideToolTip');
        }
        else {
            $("#sliderInflation").exponentialslider("finalvalue", "0");
            sliderToolTipInflation.html("");
            $(sliderToolTipInflation).addClass('HideToolTip');
        }
    })
    //  });
}
function CalculateOnLoad(methodInputYear, methodInputPresentValue, methodInputInflationPercent, methodInputLumpsu, methodInputRiskprofile) {
    methodInput = new Object();
    methodInput.year = methodInputYear;
    methodInput.presentValue = methodInputPresentValue;
    methodInput.inflationPercent = methodInputInflationPercent;
    methodInput.lumpsum = methodInputLumpsu;
    methodInput.riskprofile = methodInputRiskprofile;
    $.ajax({
        type: "POST",
        async: false,
        url: familyUrl + 'GetExpectedReturn',
        data: JSON.stringify(methodInput),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resp) {
            //alert("success resp "+JSON.stringify(resp));
            jsonData = JSON.parse(resp);
            var result = true;
            var fval = Math.round(jsonData[0].FutureValue);
            var ysip = Math.round(jsonData[0].AnnualSIP);
            var msip = Math.round(parseFloat(jsonData[0].AnnualSIP) / 12);

            $("#finalvalue").text(formatNumber(fval));
            $("#monthlysip").text(formatNumber(msip));
            $("#yearlysip").text(formatNumber(ysip));
            monthlySip = jsonData[0].AnnualSIP / 12;
            equity = Math.round((jsonData[0].Equity * monthlySip / 100));
            shortTerm = Math.round((jsonData[0].ShortTerm * monthlySip / 100));
            longTerm = Math.round((jsonData[0].LongTerm * monthlySip / 100));
            gold = Math.round((jsonData[0].Gold * monthlySip / 100));

            plannedYear = $('#txtYears').val();
            $("#yearPlanned").text(plannedYear);
            if (fval >= 0 || ysip >= 0 || msip >= 0)
                result = true;
            else if (fval < 0 || ysip < 0 || msip < 0)
                result = false;

            else {
                alert('You already have enough investment to achieve this goal. Please replan your goal.');
            }
        },
        beforeSend: function () {
            idealake_website_loaderManagement.init();
            // Loader();
        },
        complete: function () {
            // Code to hide spinner.
            idealake_website_loaderManagement.LoaderEnd();
        },
        error: function (xhr, status, e) {
            idealake_website_loaderManagement.LoaderEnd();
            alert("There seems to be issue with network.");
            result = false;
        }
    });

}
function Calculate(methodInputYear, methodInputPresentValue, methodInputInflationPercent, methodInputLumpsu, methodInputRiskprofile) {
    methodInput = new Object();
    methodInput.year = methodInputYear;
    methodInput.presentValue = methodInputPresentValue;
    methodInput.inflationPercent = methodInputInflationPercent;
    methodInput.lumpsum = methodInputLumpsu;
    methodInput.riskprofile = methodInputRiskprofile;
    $.ajax({
        type: "POST",
        async: false,
        url: familyUrl + 'GetExpectedReturn',
        data: JSON.stringify(methodInput),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resp) {
            //alert("success resp "+JSON.stringify(resp));
            jsonData = JSON.parse(resp);
            var result = true;
            var fval = Math.round(jsonData[0].FutureValue);
            var ysip = Math.round(jsonData[0].AnnualSIP);
            var msip = Math.round(parseFloat(jsonData[0].AnnualSIP) / 12);

            $("#finalvalue").text(formatNumber(fval));
            $("#monthlysip").text(formatNumber(msip));
            $("#yearlysip").text(formatNumber(ysip));
            monthlySip = jsonData[0].AnnualSIP / 12;
            equity = Math.round((jsonData[0].Equity * monthlySip / 100));
            shortTerm = Math.round((jsonData[0].ShortTerm * monthlySip / 100));
            longTerm = Math.round((jsonData[0].LongTerm * monthlySip / 100));
            gold = Math.round((jsonData[0].Gold * monthlySip / 100));

            plannedYear = $('#txtYears').val();
            $("#yearPlanned").text(plannedYear);
            if (fval >= 0 || ysip >= 0 || msip >= 0)
                result = true;
            else if (fval < 0 || ysip < 0 || msip < 0)
                result = false;
            if (result) {

                $('.calculatorInput').hide();
                $('.calculatorOutput').show();
                return false;
            }
            else {
                alert('You already have enough investment to achieve this goal.Please replan your goal.');
            }
        },
        beforeSend: function () {
            idealake_website_loaderManagement.init();
            // Loader();
        },
        complete: function () {
            // Code to hide spinner.
            idealake_website_loaderManagement.LoaderEnd();
        },
        error: function (xhr, status, e) {

            alert("There seems to be issue with network.");
            result = false;
        }
    });

}
var sipBanks = [];
function GetUserScheme(type, option) {
    //  var schemetype = $("input[name=schemesType]:checked").val();

    var pData = { "SchemeType": type };
    $.ajax({
        type: "POST",
        async: false,
        url: familyUrl + 'GetRecommendedSchemes',
        data: JSON.stringify(pData),
        contentType: 'application/json; charset=utf-8',
        success: function (resp) {
            if (resp != '') {
                $('.schemeResultContainer').html('');
                var json = JSON.parse(resp);
                var htmlData = '';
                if (option == 'transaction') {

                    $('.family-data').html('')
                    htmlData += '<div>'
                    htmlData += '        <table cellspacing="0" rules="all" border="1" id="grdJobDetails" style="border-collapse: collapse; float:left; width: 100% ">                                                                                                                                                         '
                    htmlData += '            <tbody>                                                                                                                                                                                                                                                                                                                 '
                    htmlData += '                <tr>                                                                                                                                                                                                                                                                                                                '
                    htmlData += '                    <th scope="col" style="width:30%;">Fund Name SIP</th>                                                                                                                                                                                                                                                                            '
                    htmlData += '                    <th scope="col" style="width:10%;">Monthly SIP&nbsp;&nbsp;<span>(<i class="fa"><img src="/_catalogs/masterpage/images/icons/rupee-white.png"></i>)</span></th>                                                                                                                                                                                                                                                                                   '
                    htmlData += '                    <th scope="col" style="width:10%;">Latest NAV&nbsp;&nbsp;<span>(<i class="fa"><img src="/_catalogs/masterpage/images/icons/rupee-white.png"></i>)</span></th>                                                                                                                                                                                                                                                                                   '
                    htmlData += '                </tr>    '
                }
                for (i = 0; i < json.length; i++) {
                    if (option == '') {
                        $('.schemeResultContainer').append(PopulateSchemes(json[i], i))
                    } else if (option == 'transaction') {
                        htmlData += pupulateSchemeTable(json[i], i)
                    }
                }
                if (option == 'transaction') {

                    htmlData += '            </tbody>                                                                                                                                                                                                                                                                                                                '
                    htmlData += '        </table>    '
                    htmlData += '    </div>        '
                    $('.family-data').append(htmlData);
                    var amount1 = 0
                    var amount2 = 0
                    var amount3 = 0
                    var amount4 = 0
                    if ($('#invstAmt1').html() != '') {
                        amount1 = parseInt($('#invstAmt1').html())
                    }
                    if ($('#invstAmt1').html() != '') {
                        amount2 = parseInt($('#invstAmt2').html())
                    }
                    if ($('#invstAmt1').html() != '') {
                        amount3 = parseInt($('#invstAmt3').html())
                    }
                    if ($('#invstAmt1').html() != '') {
                        amount4 = parseInt($('#invstAmt4').html())
                    }
                    finalAmount = amount1 + amount2 + amount3 + amount4
                    $('#lblInvestAmt').append(finalAmount)
                }

                $('.appendButton').html('');
                var webUrl = _spPageContextInfo.webAbsoluteUrl;
                //$('.appendButton').append('<div class="fund-Invest"><div class="common_button scheme-invest-btn-align"><a class="invstNow mrR20" href="#">Invest Now</a></div><div class="common_button"><a class="invstNow" href="' + webUrl + '/financial-planning-calculators/family-solution-tool">Click to start Over</a></div> </div>');
                $('.appendButton').append('<a href="#"  class="btn-primary btn-forsite invstNow" >INVEST NOW</a><a class="btn-primary btn-forsite" href="' + webUrl + '/financial-planning-calculators/family-solution-tool">START OVER</button>');
                if ($('.calculatorOutput').length > 0) {
                    invstClickInitilize()
                }
                if (equity == 0) {
                    $('#sch0').parent().hide();
                }
                if (shortTerm == 0) {
                    $('#sch1').parent().hide();
                }
                if (longTerm == 0) {
                    $('#sch2').parent().hide();
                }
                if (gold == 0) {
                    $('#sch3').parent().hide();
                }
            }
        },
        beforeSend: function () {
            idealake_website_loaderManagement.init();
            // Loader();
        },
        complete: function () {
            // Code to hide spinner.
            idealake_website_loaderManagement.LoaderEnd();
        },
        error: function (xhr, status, e) {
            alert("There seems to be issue with network.");
        }
    });
}
function invstClickInitilize() {
    $('.invstNow').click(function () {
        var schemeType = ''
        var chkSelected = $("input:radio:checked").val();
        if (chkSelected == 'D') {
            schemeType = 'D'
        }
        else {
            schemeType = 'R'
        }
        arrGoalid = $("#hdnGoalID").val();
        encryptFamilyData(methodInputYear, methodInputPresentValue, methodInputInflationPercent, methodInputLumpsum, methodInputRiskprofile, schemeType, arrGoalid);
    })
}
function encryptFamilyData(year, presentValue, inflation, lumpsum, riskProfile, SchemeType, goalID) {
    var pdata = { "year": year, "presentValue": presentValue, "inflation": inflation, "lumpsum": lumpsum, "riskProfile": riskProfile, "schType": SchemeType, "goalID": goalID }
    $.ajax({
        type: "POST",
        async: false,
        data: JSON.stringify(pdata),
        url: '/_vti_bin/SBIMF/SBIMFTrnsService.svc/EncryptFamilyDetails',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resp) {
            var redirectLink = resp;
            if (redirectLink != "") {
                var QueryStringValues = redirectLink;
                var key = "il_familysolutioninvestment"
                redirectUrl(key, QueryStringValues);
            } else {
                alert("There seems to be network issue. Please try again.");
            }
        },
        error: function (xhr, status, e) {
            alert("There seems to be network issue. Please try again.");
        }
    });

};
var methodInputYear;
var methodInputPresentValue;
var methodInputInflationPercent;
var methodInputLumpsum;
var methodInputRiskprofile;
//Questioner And Option
$(document).ready(function () {
    HideShowSliderPropertiesOnload();
    RedirectLinksFinancialPlanningCalculatorsPage();
    GetParameterValues();
    if ($('.calculatorOutput').length > 0) {
        var FormattedValue = CommaSeperatedNumber($('#txtAmount').val().trim());
        $('#txtAmount').val(FormattedValue);
        methodInputYear = $('#txtYears').val().trim();
        methodInputPresentValue = $('#txtAmount').val().replace(/,/g, "").trim();
        methodInputInflationPercent = $('#txtInflation').val().trim();
        methodInputLumpsum = $('#txtlumpsum').val().replace(/,/g, "").trim();
        methodInputRiskprofile = idealake_cpManager.GetCookie('il_riskAssestmentScore');
    }
    $('.calculatorOutput').hide();
    $('.SchemeInvest').hide();

    var data = getParameterByName("GoalID");
    if ($('.questionnairecontainer').length > 0) {
        LoadQuestions();
    }
    else {

    }
    $('.questionSubmit').click(function () {

        var category = "Financial Planning Calculators";
        var label = "";
        var goalId = idealake_cpManager.GetCookie('il_homePageGoalID');
        if (goalId == null || typeof goalId == 'undefined') {
            label = "Reassess Risk Profile";
        } else {
            label = GetGoalName(goalId)
        }
        var action = "submit questionaire";
        var events = "submit questionaire";
        var value = "";
        EventRecommendationWithoutRedirection(category, label, action, events, value);
        methodInput = new Array();
        $('.questionnairecontainer .questn-box.clearfix').each(function () {
            temp = new Object();
            temp["QuestionID"] = $(this).find('.questn-right').attr('data-questionid');
            $(this).find('input[type=radio]').each(function () {
                var currentRad = $(this);
                var checked = $(this).prop("checked")
                if (checked) {
                    temp["Option"] = $(this).val();
                }
            });

            methodInput.push(temp);
        });
        SendAnswers(methodInput);

    });

    $('.goalCalculate').click(function () {
        $("#s4-workspace").animate({
            scrollTop: 0
        }, "slow");
        if (Validate()) {
            methodInputYear = $('#txtYears').val().trim();
            methodInputPresentValue = $('#txtAmount').val().replace(/,/g, "").trim();
            methodInputInflationPercent = $('#txtInflation').val().trim();
            methodInputLumpsum = $('#txtlumpsum').val().replace(/,/g, "").trim();
            methodInputRiskprofile = idealake_cpManager.GetCookie('il_riskAssestmentScore');
            Calculate(methodInputYear, methodInputPresentValue, methodInputInflationPercent, methodInputLumpsum, methodInputRiskprofile);
            var user = idealake_website_userManagement.UserName();
            if (user != undefined) {
                $('#userName').html(user);
            }
            else {
                $('#userName').html('Investor');

            }
            $('.SchemeInvest').show();

        }
    });
    if ($('.calculatorContainer').length > 0) {
        Sliders();

    }

    $("#txtInflation").change(function () {
        var value = $(this).val();
        if (value != '') {
            if (value.indexOf('.') >= 0) {
                var res = $(this).val().split(".");
                num = res[0];
                if (res[1].length > 2)
                    $(this).val(Number($(this).val()).toFixed(2));
            }
        }
        else {
            num = value;
        }
    });
    $('.number').each(function () {
        $(this).keypress(function (event) {
            return isNumber(event, this)
            return false;
        });
    });
    function isNumber(evt, element) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode == 45) {
            alert('No Negative Values Allowed');
            return false;
        }
        else {
            if (
                (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
                (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
                (charCode != 0 && charCode != 8) &&
                (charCode < 48 || charCode > 57))
                return false;
        }
        return true;
    }


    $('input:radio[name="schemesType"]').change(
    function () {
        var schemetype = $(this).val()
        GetUserScheme(schemetype, '');
    });

    $('.riskcheck .risk-div').each(function () {
        $(this).click(function () {
            $('.riskcheck').attr('href', '#');
            var goald = $(this).attr('data-goalid');
            var events = $(this).attr('data-event')
            var category = $(this).attr('data-category')
            var action = $(this).attr('data-action')
            var label = $(this).attr('data-label') + "||" + $('#inSearch_on').text()
            var value = "";
            EventRecommendationWithoutRedirection(category, label, action, events, value);

            var riskprofile = idealake_cpManager.GetCookie('il_riskAssestmentScore');
            idealake_cpManager.AddCookie(goald, 'il_homePageGoalID');
            if (riskprofile != null) {

                var redirect = GetCalculatorPageURL(goald);
                window.location = redirect;
            }
            else {
                window.location = "/en-us/financial-planning-calculators/family-solution-tool/risk-assessment";
            }

        });
    });
    if ($(".riskcheckRghtDisable").length > 0) {
        $(".riskcheckRghtDisable").on("contextmenu", function () {
            alert("Right click is disabled");
            return false;
        });
    }

    $('#startGoal').click(function () {
        $(this).attr('href', '#');
        var goald = $(this).attr('data-goalid');
        var events = $(this).attr('data-event')
        var category = $(this).attr('data-category')
        var action = $(this).attr('data-action')
        var label = $(this).attr('data-label')
        var value = "";
        EventRecommendationWithoutRedirection(category, label, action, events, value);

        var riskprofile = idealake_cpManager.GetCookie('il_riskAssestmentScore');
        if (riskprofile != null) {

            var redirect = GetCalculatorPageURL(goald);
            window.location = redirect;
        }
        else {
            window.location = "/en-us/financial-planning-calculators/family-solution-tool/risk-assessment?GoalID=" + goald;
        }
    });
    if (data != "") {
        if ($('.fst-disclaimer').length == 0) {
            GetInflation(data);
        }
    }
    if ($('#family-Disclaimer').length > 0) {
        $('#family-Disclaimer').click(function () {
            var category = $(this).attr('data-category');
            var label = $(this).attr('data-label');
            var action = $(this).attr('data-action');
            var events = $(this).attr('data-event');
            var value = "";
            EventRecommendationWithoutRedirection(category, label, action, events, value);
            idealake_website_loaderManagement.init();
            methodInputRiskprofile = idealake_cpManager.GetCookie('il_riskAssestmentScore');
            var homepageCookieGoalID = idealake_cpManager.GetCookie('il_homePageGoalID');
            if (homepageCookieGoalID == null) {
                window.location = "/en-us/financial-planning-calculators/family-solution-tool"
            }
            else {
                if (methodInputRiskprofile != null) {

                    var redirect = GetCalculatorPageURL(homepageCookieGoalID);
                    window.location = redirect;
                }
                else {
                    window.location = "/en-us/financial-planning-calculators/family-solution-tool/risk-assessment";
                }
            }
        })
    }
    if ($('.dvInput').length > 0) {
        CalculateOnLoad(methodInputYear, methodInputPresentValue, methodInputInflationPercent, methodInputLumpsum, methodInputRiskprofile);
    }
    //var cookieValue = idealake_cpManager.GetCookie('il_riskAssestmentScore');
    if ($('.risk-reassis').length > 0) {
        if (cookieValue == null) {
            $('.risk-reassis').hide();
        }
        else {
            $('.risk-reassis').show();
        }
    }
    $('.risk-reassis').click(function () {
        var getQueryStringValue = GetAllParameterValues();
        if (getQueryStringValue != null) {
            window.location = '/en-us/financial-planning-calculators/family-solution-tool/risk-assessment?GoalID=' + getQueryStringValue
        }
        else {
            window.location = '/en-us/financial-planning-calculators/family-solution-tool/risk-assessment';
        }
    })
    $('.risk').each(function () {
        $(this).click(function () {
            //var riskData = $(this).attr('data-riskData')
            var getQueryStringValue = GetAllParameterValues();
            if (getQueryStringValue != null) {
                window.location = '/en-us/financial-planning-calculators/family-solution-tool/risk-assessment?GoalID=' + getQueryStringValue
            }
            else {
                window.location = '/en-us/financial-planning-calculators/family-solution-tool/risk-assessment';
            }
        })
    })

    if ($('.assis').length > 0) {
        if (cookieValue == null) {
            $('.assis').show()
            $('.reAssis').hide()
        }
        else {
            $('.assis').hide()
            $('.reAssis').show()
        }
    }

});

function GetGoalName(goalId) {
    var goalName = '';
    switch (goalId) {
        case '1':
            goalName = 'Retirement';
            break;
        case '2':
            goalName = 'Dream House';
            break;
        case '3':
            goalName = 'Child Education';
            break;
        case '4':
            goalName = 'Child Wedding';
            break;
        case '5':
            goalName = 'Vacation';
            break;
        case '6':
            goalName = 'Apna Goal';
            break;
    }
    return goalName;
}


function RedirectLinksFinancialPlanningCalculatorsPage() {
    $(".btnFamilySolutionTool").click(function () {
        window.location.href = "/en-us/financial-planning-calculators/family-solution-tool";
        return false;
    });
    $(".btnSystematicInvestmentPlanner").click(function () {
        window.location.href = "/en-us/financial-planning-calculators/systematic-investment-planner";
        return false;
    })
    $(".btnSWPCalculator").click(function () {
        window.location.href = "/en-us/financial-planning-calculators/swp-calculator";
        return false;
    })
    $(".btnReturnValueCalculator").click(function () {
        window.location.href = "/en-us/financial-planning-calculators/return-value-calculator";
        return false;
    })
}

function HideShowSliderPropertiesOnload() {
    $(".sliderYearsMin").css("display", "none");
    $(".sliderAmountMin").css("display", "none");
    $(".sliderInflationMin").css("display", "none");
    $(".sliderLumpsumMin").css("display", "none");
}