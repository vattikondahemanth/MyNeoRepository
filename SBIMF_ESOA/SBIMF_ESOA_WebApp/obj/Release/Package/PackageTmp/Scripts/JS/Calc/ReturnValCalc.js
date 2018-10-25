var sliderToolTipInvestment;
var sliderToolTipDuration;
var commonObj = new CommonJS();
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
function InitializeCommaSeperatedNumbers() {
    if ($('.CommaSeperated').length > 0) {
        $('.CommaSeperated').each(function (index) {
            $(this).keyup(function (e) {
                var Number = $(this).val();
                Number = Number.toString().replace(/,/g, "");
                var CommaSeperated = CommaSeperatedNumber(Number);
                $(this).val(CommaSeperated).change();
            });

        });
    };
}
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
function clearFields() {
    $('.yearsInvestment').html('');
    $('.principalinvested').html('');
    $('.finalresult').html('');
    $('#txtamt').val('');
    $('#txtyears').val('');
};
function ResetSliders() {
    $('.slider').each(function () {

        $(this).exponentialslider("value", $(this).exponentialslider("option", "min"));
    });
    sliderToolTipDuration.html('1 Yr');
}
function InputData(requestData) {
    $.ajax({
        type: "POST",
        dataType: "json",
        async: true,
        url: "",
        data: JSON.stringify(requestData),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (resp) {
            var jsonData = JSON.parse(resp);
        },
        error: function (xhr, status, e) {
            $('.finalMessage').html("There seems to be issue with network.");
        }
    });
}
function CalculateSIP() {
    var lump = Getdata();
    var InvAmt = parseFloat(lump.InvestmentAmount);
    var NoOfYears = parseFloat(lump.Years);
    var Rate = parseFloat(lump.Ratio);
    if (!isNaN(Rate)) {
        Rate /= 100;
    }
    var RateOfReturn = (1 + (Rate / 12));
    var ResultYears = (1 + (NoOfYears * 12));
    var powres = Math.pow(RateOfReturn, ResultYears);
    var RateRes = (powres - 1);
    var result = ((InvAmt * (RateRes) / (Rate / 12))) - InvAmt;
    $('.yearsInvestment').html(formatNumber(NoOfYears));
    $('.principalinvested').html(formatNumber(InvAmt * (NoOfYears * 12)));
    $('.finalresult').html(formatNumber(result.toFixed(2)));
    //ScrollPageDown();
}
function CalculateLumpSum() {
    var SIP = Getdata();
    var InvAmt = parseFloat(SIP.InvestmentAmount);
    var NoOfYears = parseFloat(SIP.Years);
    var Rate = parseFloat(SIP.Ratio);
    if (!isNaN(Rate)) {
        Rate /= 100;
    }
    var result = InvAmt * (Math.pow((1 + Rate), NoOfYears));
    $('.yearsInvestment').html(formatNumber(NoOfYears));
    $('.principalinvested').html(formatNumber(InvAmt));
    $('.finalresult').html(formatNumber(result.toFixed(2)));
    //ScrollPageDown();
}
function ScrollPageDown() {
    if (window.innerWidth > 900) {
        $("#s4-workspace").animate({
            scrollTop: ($(document).height() - 300)
        }, 1000);
    }
    else {

        $("#s4-workspace").animate({
            scrollTop: ($(document).height() + 200)
        }, 1000);
    }
}
function Getdata() {
    var getdata = {};
    var investmentAmount = $('#txtamt').val().replace(/,/g, "");
    var years = $('#txtyears').val();
    var ratio = $('#ddlexpectedReturn :selected').text();
    getdata.InvestmentAmount = investmentAmount;
    getdata.Years = years;
    getdata.Ratio = ratio;
    return getdata;
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
    var encryptedPAN = "";
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
function HideShowInvestmentRange(x, range) {
    if (range == "T") {
        var min = x < 3000;
        var max = x > 80000;
    }
    else if (range == "L") {
        var min = x < 300000;
        var max = x > 8000000;
    }
    else if (range == "C") {
        var min = x < 30000000;
        var max = x > 800000000;
    }
    if (min) { $(".sliderAmtMin").css("display", "none"); }
    else { $(".sliderAmtMin").css("display", "block"); }
    if (max) { $(".sliderAmtMax").css("display", "none"); }
    else { $(".sliderAmtMax").css("display", "block"); }
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

    $(".sliderAmtMin").css("display", "none");
    sliderToolTipInvestment = $('<span id="sliderToolTipInvestment" class="sliderTooltip"></span>');
    sliderToolTipDuration = $('<span id="sliderToolTipDuration" class="sliderTooltip"></span>');
    var formattedValue = CommaSeperatedNumber(1000);
    $('#txtamt').val(formattedValue);
    $('#txtyears').val(1);
    $('#btnCalculate').click(function () {
        var investortype = $('.investType:checked').val();
        var investamount = $('.InvesAmount:checked').val();
        var category = "Financial Planning Calculators";
        var label = "calculate|" + investortype + "|in " + investamount + "";
        var action = "returns value calculator";
        var events = "calculate return value";
        var value = "";
        //EventRecommendationWithoutRedirection(category, label, action, events, value);

        Getdata();
        if ($("#rdbSip").is(':checked')) {
            CalculateSIP();
            $("#CalcInvestmentType").text("Enter your PAN to start your SIP");
        }
        else {
            $("#CalcInvestmentType").text("Enter your PAN to start investing");
            CalculateLumpSum();
        }
        $("#dvResult").css("display", "block");

        var webUrl = webAbsoluteUrl;// = _spPageContextInfo.webAbsoluteUrl;
        if ($('.appendFund').length > 0) {
            GetRcmdFund(webUrl);
        }
        $('.calculatorMain').hide();
        $('.calculatedresultbox').show();

        $("#dvResult").scrollView();
        //return false;
    });
    $("#rdbSip").change(function () {
        $(sliderToolTipInvestment).removeClass('HideToolTip');
        $(sliderToolTipDuration).removeClass('HideToolTip');
        $(".calculatedresultbox").hide();
        $(".calculatorMain").show();
        var formattedValue = CommaSeperatedNumber(1000);
        $('#txtamt').val(formattedValue);
        $('#txtyears').val(1);
        $('#rdbthousands').prop('checked', true);
        $("#txtamt").attr('maxlength', '5')
        $('.calculatedresultbox').hide();
        $(".Amount .slider-min").html("1K");
        $(".Amount .slider-max").html("99K");
        ResetSliders();
        $(function () {
            $("#sliderInvestment").exponentialslider({
                logicalmin: 1000,
                logicalmax: 100000,
                logicalstep: 1000,
                slidertype: 'exponential',
                exponentialslide: onSlider2Move
            });
            $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
            var InitialValue = 1000;
            var DisplayInitailValue = numDifferentiation(InitialValue);
            sliderToolTipInvestment.html(DisplayInitailValue);

            function onSlider2Move(e, ui) {
                var x = ui.finalvalue;
                if (x == 100000) {
                    x -= 1000;
                }
                var formattedValue = CommaSeperatedNumber(x);
                $('#txtamt').val(formattedValue);
                var SliderValue = x;
                var DisplayedValue = numDifferentiation(SliderValue);
                sliderToolTipInvestment.html(DisplayedValue);
            }
        });
    });
    $("#rdblumpsum").change(function () {

        $(sliderToolTipInvestment).removeClass('HideToolTip');
        $(sliderToolTipDuration).removeClass('HideToolTip');
        $(".calculatedresultbox").hide();
        $(".calculatorMain").show();
        $('#rdbthousands').prop('checked', true);
        var formattedValue = CommaSeperatedNumber(1000);
        $('#txtamt').val(formattedValue);
        $('#txtyears').val(1);
        $(".Amount .slider-min").html("1K");
        $(".Amount .slider-max").html("99K");
        ResetSliders();
        $("#txtamt").attr('maxlength', '5')
        $('.calculatedresultbox').hide();
        $(function () {
            $("#sliderInvestment").exponentialslider({
                logicalmin: 1000,
                logicalmax: 100000,
                logicalstep: 1000,
                slidertype: 'exponential',
                exponentialslide: onSlider2Move
            });
            $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
            var InitialValue = 1000;
            var DisplayInitailValue = numDifferentiation(InitialValue);
            sliderToolTipInvestment.html(DisplayInitailValue);

            function onSlider2Move(e, ui) {
                var x = ui.finalvalue;
                if (x == 100000) {
                    x -= 1000;
                }
                var formattedValue = CommaSeperatedNumber(x);
                $('#txtamt').val(formattedValue);
                var SliderValue = x;
                var DisplayedValue = numDifferentiation(SliderValue);
                sliderToolTipInvestment.html(DisplayedValue);
            }
        });
    });
    $(".text-field").keypress(function (event) {
        return isNumber(event, this)
        return false;
    });
    $(function () {
        sliderToolTipInvestment.html('');
        $("#sliderInvestment").exponentialslider({
            logicalmin: 1000,
            logicalmax: 100000,
            logicalstep: 1000,
            slidertype: 'exponential',
            exponentialslide: onSlider2Move
        });
        $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
        var InitialValue = 1000;
        var DisplayInitailValue = numDifferentiation(InitialValue);
        sliderToolTipInvestment.html(DisplayInitailValue);

        function onSlider2Move(e, ui) {
            var x = ui.finalvalue;
            if (x == 100000) {
                x -= 1000;
            }
            var formattedValue = CommaSeperatedNumber(x);
            $('#txtamt').val(formattedValue);
            var SliderValue = x;
            var DisplayedValue = numDifferentiation(SliderValue);
            sliderToolTipInvestment.html(DisplayedValue);
            $('.erroramt').html('');
            $(sliderToolTipInvestment).removeClass('HideToolTip');
            HideShowInvestmentRange(x, "T");
        }

    });
    $(function () {
        sliderToolTipDuration.html('');
        $("#sliderDuration").exponentialslider({
            logicalmin: 1,
            logicalmax: 30,
            logicalstep: 1,
            slidertype: 'linear',
            exponentialslide: onSlider2Move
        });
        $('#sliderDuration').find('.ui-slider-handle').append(sliderToolTipDuration);
        var InitialDurationValue = '1 Yr';
        sliderToolTipDuration.html(InitialDurationValue);
        function onSlider2Move(e, ui) {
            var x = ui.finalvalue;
            $('#txtyears').val(x);
            var DurationValue = x + ' Yrs';
            if (x == 1) DurationValue = x + ' Yr';
            sliderToolTipDuration.html(DurationValue);
            $('.erroryears').html('');
            $(sliderToolTipDuration).removeClass('HideToolTip');

            if (x < 3) { $(".sliderYearsMin").css("display", "none"); }
            else { $(".sliderYearsMin").css("display", "block"); }
            if (x > 25) { $(".sliderYearsMax").css("display", "none"); }
            else { $(".sliderYearsMax").css("display", "block"); }

        }
        $("#txtyears").change(function () {
            var data = this.value;
            if (data != "") {
                $(sliderToolTipDuration).removeClass('HideToolTip');
                $("#sliderDuration").exponentialslider("finalvalue", data);
                var DurationValue = data + ' Yrs';
                if (data == 1) DurationValue = data + ' Yr';
                sliderToolTipDuration.html(DurationValue);
                $('.erroryears').html('');
            }
            else {
                $("#sliderDuration").exponentialslider("finalvalue", "0");
                sliderToolTipDuration.html("");
                $(sliderToolTipDuration).addClass('HideToolTip');
            }
        })
    });
    $('#rdbthousands').change(function () {
        $(".calculatedresultbox").hide();
        $(".calculatorMain").show();
        sliderToolTipInvestment.html('');
        var formattedValue = CommaSeperatedNumber(1000);
        $('#txtamt').val(formattedValue);
        $('#txtyears').val(1);
        $(".Amount .slider-min").html("1K");
        $(".Amount .slider-max").html("99K");
        ResetSliders();
        $(".sliderAmtMin").hide();
        $('#txtamt').attr('maxlength', '5');
        $("#sliderInvestment").exponentialslider({
            logicalmin: 1000,
            logicalmax: 100000,
            logicalstep: 1000,
            slidertype: 'exponential',
            exponentialslide: onSlider2Move
        });
        $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
        var InitialValue = 1000;
        var DisplayInitailValue = numDifferentiation(InitialValue);
        sliderToolTipInvestment.html(DisplayInitailValue);

        function onSlider2Move(e, ui) {
            var x = ui.finalvalue;
            if (x == 100000) {
                x -= 1000;
            }
            var formattedValue = CommaSeperatedNumber(x);
            $('#txtamt').val(formattedValue);

            var SliderValue = x;
            var DisplayedValue = numDifferentiation(SliderValue);
            sliderToolTipInvestment.text(DisplayedValue);
            $('.erroramt').html('');
            $(sliderToolTipInvestment).removeClass('HideToolTip');
            HideShowInvestmentRange(x);
        }
    });
    $('#rdblacs').change(function () {
        $(".calculatedresultbox").hide();
        $(".calculatorMain").show();
        sliderToolTipInvestment.html('');
        $(".Amount .slider-min").html("1L");
        $(".Amount .slider-max").html("99L");
        var formattedValue = CommaSeperatedNumber(100000);
        $('#txtamt').val(formattedValue);
        $('#txtyears').val(1);
        ResetSliders();
        $(".sliderAmtMin").hide();
        $('#txtamt').attr('maxlength', '7');
        $("#sliderInvestment").exponentialslider({
            logicalmin: 100000,
            logicalmax: 10000000,
            logicalstep: 100000,
            slidertype: 'exponential',
            exponentialslide: onSlider2Move
        });
        $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
        var InitialValue = 100000;
        var DisplayInitailValue = numDifferentiation(InitialValue);
        sliderToolTipInvestment.html(DisplayInitailValue);

        function onSlider2Move(e, ui) {
            var x = ui.finalvalue;
            if (x == 10000000) {
                x -= 100000;
            }
            var formattedValue = CommaSeperatedNumber(x);
            $('#txtamt').val(formattedValue);
            var SliderValue = x;
            var DisplayedValue = numDifferentiation(SliderValue);
            sliderToolTipInvestment.html(DisplayedValue);
            $('.erroramt').html('');
            $(sliderToolTipInvestment).removeClass('HideToolTip');
            HideShowInvestmentRange(x, "L");
        }
    });
    $('#rdbcrores').change(function () {
        $(".calculatedresultbox").hide();
        $(".calculatorMain").show();
        sliderToolTipInvestment.html('');
        $(".Amount .slider-min").html("1Cr");
        $(".Amount .slider-max").html("99Cr");
        var formattedValue = CommaSeperatedNumber(10000000);
        $('#txtamt').val(formattedValue);
        $('#txtyears').val(1);
        ResetSliders();
        $(".sliderAmtMin").hide();
        $('#txtamt').attr('maxlength', '9')
        $("#sliderInvestment").exponentialslider({
            logicalmin: 10000000,
            logicalmax: 1000000000,
            logicalstep: 10000000,
            slidertype: 'exponential',
            exponentialslide: onSlider2Move
        });
        $('#sliderInvestment').find('.ui-slider-handle').append(sliderToolTipInvestment);
        var InitialValue = 10000000;
        var DisplayInitailValue = numDifferentiation(InitialValue);
        sliderToolTipInvestment.html(DisplayInitailValue);

        function onSlider2Move(e, ui) {
            var x = ui.finalvalue;
            if (x == 1000000000) {
                x -= 10000000;
            }
            var formattedValue = CommaSeperatedNumber(x);
            $('#txtamt').val(formattedValue);
            var SliderValue = x;
            var DisplayedValue = numDifferentiation(SliderValue);
            sliderToolTipInvestment.html(DisplayedValue);
            $('.erroramt').html('');
            $(sliderToolTipInvestment).removeClass('HideToolTip');
            HideShowInvestmentRange(x, "C");
        }
    });
    $("#txtamt").change(function () {
        var data = this.value;
        if (data != "") {
            var numberWithoutComma = data.toString().replace(/,/g, "");
            data = parseFloat(numberWithoutComma);
            $("#sliderInvestment").exponentialslider("finalvalue", data);
            var DisplayInitailValue = numDifferentiation(data);
            sliderToolTipInvestment.html(DisplayInitailValue);
            $(this).val(CommaSeperatedNumber(data));
            $('.erroramt').html('');
            $(sliderToolTipInvestment).removeClass('HideToolTip');
        }
        else {
            $("#sliderInvestment").exponentialslider("finalvalue", "0");
            sliderToolTipInvestment.html("");
            $(sliderToolTipInvestment).addClass('HideToolTip');
        }
    });
    $("#submitPAN").click(function () {
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
    $('.toUpperCase').keyup(function () {
        $(this).val($(this).val().toUpperCase());
    });
});