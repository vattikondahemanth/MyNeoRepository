var commonObj = new CommonJS();
        var $table = $('table.table-swp'),
            $bodyCells = $table.find('tbody tr:first').children(),
            colWidth;

        $(window).resize(function () {
            colWidth = $bodyCells.map(function () {
                return $(this).width();
            }).get();

            $table.find('thead tr').children().each(function (i, v) {
                $(v).width(colWidth[i]);
            });
        }).resize(); // Trigger resize handler

        var AllDates = '';
        var NavHtml = '';
        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var CiiData = null;

        var loader = {};
        loader = {
            start: function () {
                $('#SiteLoader').addClass('overlay-loader').append($('<div>').addClass('loader'));
            },
            stop: function () {
                $('#SiteLoader').removeClass('overlay-loader').empty();
            }
        }

        $.fn.scrollView = function () {
            return this.each(function () {
                $('html, body').animate({
                    scrollTop: $(this).offset().top
                }, 1000);
            });
}


$(document).ready(function () {
    loader.stop();


    forgeryId = $("#forgeryToken").val();

            //$('.disclaimer').hide();
            fillYears();
            CiiData = GetCIIData();
            $('#ddlScheme').val('D103G`E`02-Jan-2013');
            $("#txtInvDate").datepicker({
                dateFormat: 'dd-mm-yy',
                changeMonth: true,
                changeYear: true,
                yearRange: '-100y:c+nn',
                maxDate: new Date,
                onSelect: function (date) {
                    var splitDate = $('#ddlScheme :selected').val().split('`')[2];
                    var startNavDate = splitDate.split('-')[2] + "/" + splitDate.split('-')[1] + "/" + splitDate.split('-')[0]
                    var invDateSplit = $('#txtInvDate').val().split('-');
                    var invDate = new Date(invDateSplit[2] + '/' + invDateSplit[1] + '/' + invDateSplit[0]);
                    var SchemeDate = new Date(startNavDate);
                    var blockScheme = ['080B', 'D080B', '074G', 'D074G'];
                    var schCode = $('#ddlScheme :selected').val().split('`')[0];
                    if ($.inArray(schCode, blockScheme) < 0) {
                        if (invDate < SchemeDate) {
                            alert('Investment date should be greater than or equal to ' + startNavDate + ' for selected scheme');
                            var requiredDate = new Date(startNavDate);
                            var dd = requiredDate.getDate();
                            var mm = requiredDate.getMonth() + 1; //January is 0!

                            var yyyy = requiredDate.getFullYear();
                            if (dd < 10) {
                                dd = '0' + dd;
                            }
                            if (mm < 10) {
                                mm = '0' + mm;
                            }
                            requiredDate = dd + '-' + mm + '-' + yyyy;
                            $('#txtInvDate').val(requiredDate);
                        }
                    }
                    else {
                        if (schCode == '080B' || schCode == 'D080B') {
                            if (invDate < new Date('2014-07-17')) {
                                alert('Investment date should be greater than or equal to 17-07-2014 for selected scheme');
                                //$('#txtInvDate').val('');
                                $('#txtInvDate').val('17-07-2014');
                            }

                        }
                        else {
                            if (invDate < new Date('2009-11-23')) {
                                alert('Investment date should be greater than or equal to 23-11-2009 for selected scheme');
                                //$('#txtInvDate').val('');
                                $('#txtInvDate').val('23-11-2009');
                            }
                        }
                    }
                }
            });
            var currDate = new Date();
            var currMonth = currDate.getMonth();
            $('#ddlSWPStartMonth').val(currMonth);
            $('#ddlSWPEndMonth').val(currMonth);
            $('#ddlInvMonth').val(currMonth);
            var toDayDateStr = "" + currDate.getDate();
            var toDayMonthStr = "" + (currDate.getMonth() + 1);
            var stringDate = (toDayDateStr.length > 1 ? currDate.getDate() : "0" + currDate.getDate()) + "-" + (toDayMonthStr.length > 1 ? currDate.getMonth() + 1 : "0" + (currDate.getMonth() + 1)) + "-" + (currDate.getFullYear() - 2);
            $("#txtInvDate").val(stringDate);
            $('#ddlSWPEndDay ').prop('disabled', true);
            $('#ddlSWPStartDay').change(function () {
                $('#ddlSWPEndDay ').prop('disabled', false);
                $('#ddlSWPEndDay :selected').text($('#ddlSWPStartDay :selected').text());
                $('#ddlSWPEndDay ').prop('disabled', true);
            });
            $('#ddlScheme').change(function () {
                $('#txtInvDate').val('');
            })
            $('#btnCalcualte').click(function () {
                loader.start();
                setTimeout(function () {
                    if (Validate()) {
                        AllDates = '';
                        var swpStartDay = $('#ddlSWPStartDay :selected').text();
                        var swpStartMonth = $('#ddlSWPStartMonth :selected').text();
                        var swpStartYear = $('#ddlSWPStartYear :selected').text();
                        var frmDate = new Date(swpStartYear + '/' + swpStartMonth + '/' + swpStartDay);
                        var swpEndDay = $('#ddlSWPEndDay :selected').text();
                        var swpEndMonth = $('#ddlSWPEndMonth :selected').text();
                        var swpEndYear = $('#ddlSWPEndYear :selected').text();
                        var toDate = new Date(swpEndYear + '/' + swpEndMonth + '/' + swpEndDay);
                        var fromMonth = frmDate.getMonth();
                        var fromDay = frmDate.getDate();
                        var fromYear = frmDate.getFullYear();
                        var numberOfMonthForSWP = monthDiff(frmDate, toDate);
                        var invDate = $('#txtInvDate').val();
                        var frequency = $('#ddlFrequency :selected').val();
                        for (var i = 0; i <= numberOfMonthForSWP; i++) {
                            if (i == 0) {
                                AllDates += (parseInt(invDate.split('-')[0]) + '-' + months[(parseInt(invDate.split('-')[1])) - 1] + '-' + invDate.split('-')[2]);
                            }
                            if (frequency == 'Quarterly') {
                                if (i % 3 == 0) {
                                    if (fromMonth == 3 || fromMonth == 5 || fromMonth == 8 || fromMonth == 10) {
                                        if (fromDay > 30) {
                                            AllDates += "," + 1 + '-' + months[fromMonth + 1] + '-' + fromYear;
                                            fromDay = 1;
                                            fromMonth = fromMonth + 1
                                        }
                                        else {
                                            AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                        }
                                    }
                                    else if (fromMonth == 1) {
                                        var isLeap = !(new Date(fromYear, 1, 29).getMonth() - 1)
                                        if (isLeap) {
                                            if (fromDay > 29) {
                                                AllDates += ',29-' + months[fromMonth + 1] + '-' + fromYear;
                                            }
                                            else {
                                                AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                            }
                                        }
                                        else {
                                            if (fromDay > 28) {
                                                AllDates += ',28-' + months[fromMonth + 1] + '-' + fromYear;
                                            }
                                            else {
                                                AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                            }
                                        }
                                    }
                                    else {
                                        AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                    }
                                }
                                if (i == numberOfMonthForSWP) {
                                    AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                }
                            }
                            else {
                                if (fromMonth == 3 || fromMonth == 5 || fromMonth == 8 || fromMonth == 10) {
                                    if (fromDay > 30) {
                                        AllDates += "," + 1 + '-' + months[fromMonth + 1] + '-' + fromYear;
                                        fromDay = 1;
                                        fromMonth = fromMonth + 1
                                    }
                                    else {
                                        AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                    }
                                }
                                else if (fromMonth == 1) {
                                    var isLeap = !(new Date(fromYear, 1, 29).getMonth() - 1)
                                    if (isLeap) {
                                        if (fromDay > 29) {
                                            AllDates += ',29-' + months[fromMonth] + '-' + fromYear;
                                        }
                                        else {
                                            AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                        }
                                    }
                                    else {
                                        if (fromDay > 28) {
                                            AllDates += ',28-' + months[fromMonth] + '-' + fromYear;
                                        }
                                        else {
                                            AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                        }
                                    }
                                }
                                else {
                                    AllDates += "," + fromDay + '-' + months[fromMonth] + '-' + fromYear;
                                }
                            }
                            if (fromMonth == 11) {
                                fromMonth = 0;
                                fromYear = fromYear + 1;
                            }
                            else {
                                fromMonth++;
                            }
                        }
                        var navDetails = GetNavData('AllDates', '');
                        GenerateHTML(navDetails, AllDates);
                        $("#appendSummary").scrollView();
                    }
                    loader.stop();
                }, 100);

                return false;
            });
            $("#txtSWPAmount").keypress(function (e) {
                if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                    return false;
                }
            });
            $("#txtInitialInvAmount").keypress(function (e) {
                if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
                    return false;
                }
            });

            $('#txtSWPAmount').bind("paste", function (e) {
                e.preventDefault();
            });

        });
        function GenerateHTML(navDetails, Dates) {
            try {
                var htmlTable = '<h3>Details Result</h3><table width="100%" cellspacing="0" border="0" class="swp-table"> <thead><tr>';
                htmlTable += '<th>Initial Investment/SWP Date</th>';
                htmlTable += '<th>NAV(Rs)</th>';
                htmlTable += '<th>Amount(Rs)</th>';
                htmlTable += '<th>Units transacted</th>';
                htmlTable += '<th>Balance Units</th>';
                htmlTable += '<th>Cost (Rs) of investment (Original NAV)</th>';
                htmlTable += '<th>Profit (Rs)</th>';
                htmlTable += '<th>Taxable Profit</th>';
                htmlTable += '<th>Tax (Rs)</th>';
                htmlTable += '<th>Net Profit (Rs)</th>';
                htmlTable += '<th>Value of Balance Units</th>';
                htmlTable += '<th>Cash Flow</th>';
                htmlTable += '<th>Net cash flow</th>';
                htmlTable += '</tr>';
                htmlTable += '</thead><tbody>';
                var SelectedScheme = $('#ddlScheme :selected').val().split('`')[1]
                if (navDetails != "" && navDetails != null && Dates != '') {
                    var arrAllNavData = [];

                    var SplitAlldates = Dates.split(',');
                    var count = 0;
                    for (var i = 0; i < SplitAlldates.length; i++) {
                        var navObj = "";
                        if (navDetails.Table.length > count) {
                            navObj = navDetails.Table[count];
                        }
                        else {
                            navObj = navDetails.Table[navDetails.Table.length - 1];
                        }
                        var navDate = navObj.NAV_DATE.split('-');
                        var finalNAVDATE = parseInt(navDate[2].split('T')[0]) + '-' + months[parseInt(navDate[1]) - 1] + "-" + navDate[0];
                        var NavData = navObj.ID + "~" + navObj.AMFICODE + "~" + navObj.NAV_DATE + "~" + navObj.NAV + "~" + navObj.SCHCODE;
                        if (finalNAVDATE != SplitAlldates[i]) {
                            var SingleData = GetNavData("BusinessDate", SplitAlldates[i])
                              var navSingleData = SingleData.Table[0];
                                NavData = navSingleData.ID + "~" + navSingleData.AMFICODE + "~" + navSingleData.NAV_DATE + "~" + navSingleData.NAV + "~" + navSingleData.SCHCODE;
                                if ($.inArray(NavData, arrAllNavData) == -1) {
                                    arrAllNavData.push(NavData)
                                    }
                        }
                        else {
                            if ($.inArray(NavData, arrAllNavData) == -1) {
                                arrAllNavData.push(NavData)
                                count++;
                            }
                        }
                    }
                    var lastBalUnits = 0;
                    var totalProfit = 0;
                    var totalTax = 0;
                    var totalNetProfit = 0;
                    var totalBalUnit = 0;
                    var totalDebtProfit = 0;
                    var amountWithdrown = 0;
                    var totalcostOfInvestment = 0;
                    var totalProfitForTavableProfit = 0;
                    var breakTotalTax = 0;
                    var htmlTableforBreak = '<tr>';
                    var breakCurrentCostInvestment = 0;
                    var breakProfit = 0;
                    var lastBreakBalUnit = 0;
                    var breakLastBalanceValue = 0;
                    var breakTotalDebtProfit = 0;
                    var breakTotalProfitForTavableProfit = 0;
                    for (var i = 0; i < arrAllNavData.length; i++) {
                        var fromDay = SplitAlldates[i].split('-')[0];
                        var fromMonth = SplitAlldates[i].split('-')[1];
                        var fromYear = SplitAlldates[i].split('-')[2];
                        htmlTable += '<tr>';
                        var nav = (parseFloat(arrAllNavData[i].split('~')[3])).toFixed(4);
                        if (i == (arrAllNavData.length - 1)) {
                            htmlTable += '<td class="col-xs-1">Total<br/>(as of latest<br/>NAV available)</td>';
                            htmlTable += '<td class="col-xs-1"></td>';
                        }
                        else {
                            htmlTable += '<td class="col-xs-1">' + fromDay + '-' + fromMonth + '-' + fromYear + '</td>';
                            htmlTable += '<td class="col-xs-1">' + nav + '</td>';
                        }
                        var unitsTransacted;
                        var costOfInvestment;
                        var initialInvestmentAmt = parseFloat($('#txtInitialInvAmount').val());
                        if (i == (arrAllNavData.length - 1)) {
                            htmlTable += '<td class="col-xs-1"></td>';
                            htmlTable += '<td class="col-xs-1"></td>';
                        }
                        else {
                            if (i == 0) {
                                htmlTable += '<td class="col-xs-1">' + formatNumber(initialInvestmentAmt) + '</td>';
                                unitsTransacted = (initialInvestmentAmt) / (parseFloat(nav));
                                costOfInvestment = initialInvestmentAmt;
                            }
                            else {
                                amountWithdrown = amountWithdrown + parseFloat($('#txtSWPAmount').val());
                                htmlTable += '<td class="col-xs-1">(' + formatNumber($('#txtSWPAmount').val()) + ')</td>';
                                unitsTransacted = "-" + (parseFloat($('#txtSWPAmount').val())) / (parseFloat(nav));
                                costOfInvestment = parseFloat(unitsTransacted) * (parseFloat(arrAllNavData[0].split('~')[3]));
                                breakCurrentCostInvestment = Math.abs(costOfInvestment);
                                totalcostOfInvestment = totalcostOfInvestment + parseFloat((Math.abs(costOfInvestment)).toFixed(3));
                            }
                            htmlTable += '<td class="col-xs-1">' + parseFloat(unitsTransacted).toFixed(3) + '</td>';
                        }
                        //---------------------Balance units start----------------
                        var balanceUnit;
                        var isbalancedUnitNegative = false;

                        if (i != (arrAllNavData.length - 1)) {
                            if (i == 0) {
                                //htmlTable += '<td>'+Math.round(unitsTransacted)+'</td>';
                                balanceUnit = unitsTransacted;
                            }
                            else {
                                //balanceUnit = Math.round(balanceUnit) + (parseFloat((unitsTransacted)))
                                lastBreakBalUnit = balanceUnit
                                balanceUnit = parseFloat(parseFloat(balanceUnit).toFixed(3)) + (parseFloat((unitsTransacted)))
                                lastBalUnits = balanceUnit;
                                //htmlTable += '<td>'+Math.round(balanceUnit)+'</td>';

                            }
                            //htmlTable += '<td>' + Math.round(balanceUnit) + '</td>';
                            htmlTable += '<td class="col-xs-1">' + balanceUnit.toFixed(3) + '</td>';
                            //below code is used for breaking loop if the balance unit is in negative
                            if (balanceUnit < 0) {
                                isbalancedUnitNegative = true;
                            }
                        }
                        else {
                            //htmlTable += '<td>' + Math.round(lastBalUnits) + '</td>';
                            htmlTable += '<td class="col-xs-1">' + lastBalUnits.toFixed(3) + '</td>';
                        }
                        if (isbalancedUnitNegative) {
                            htmlTableforBreak += '<td class="col-xs-1">Total</td><td class="col-xs-1"></td><td class="col-xs-1"></td><td class="col-xs-1"></td>';
                            htmlTableforBreak += '<td class="col-xs-1">' + lastBreakBalUnit.toFixed(3) + '</td>';
                        }
                        if (i != (arrAllNavData.length - 1)) {
                            //htmlTable += '<td>' + Math.round(Math.abs(costOfInvestment)) + '</td>';
                            htmlTable += '<td class="col-xs-1">' + formatNumber((Math.abs(costOfInvestment)).toFixed(3)) + '</td>';
                        }
                        else {
                            htmlTable += '<td class="col-xs-1"></td>';
                        }
                        if (isbalancedUnitNegative) {
                            htmlTableforBreak += '<td class="col-xs-1"></td>';
                        }
                        //---------------------Balance units End----------------
                        //---------------------Profit (Rs) E ( SWP amount- D) start----------------
                        var profit;

                        if (i == 0) {
                            htmlTable += '<td class="col-xs-1"></td>';
                        }
                        else {
                            if (i != (arrAllNavData.length - 1)) {
                                //var profit = (parseFloat($('#txtSWPAmount').val())) - (parseFloat(Math.round((Math.abs(costOfInvestment)))));
                                profit = (parseFloat($('#txtSWPAmount').val())) - (parseFloat((Math.abs(costOfInvestment)).toFixed(3)));
                                //totalProfit = totalProfit + Math.round(profit);
                                totalProfit = totalProfit + parseFloat(profit.toFixed(3));
                                if (!isbalancedUnitNegative) {
                                    breakProfit = totalProfit;
                                }
                                //htmlTable += '<td>' + Math.round(profit) + '</td>';
                                htmlTable += '<td class="col-xs-1">' + formatNumber(profit.toFixed(3)) + '</td>';
                            }
                            else {
                                htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalProfit)) + '</td>';
                            }
                            if (isbalancedUnitNegative) {
                                htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakProfit)) + '</td>';
                            }
                        }

                        //---------------------Profit (Rs) E ( SWP amount- D) End----------------
                        //---------------------Taxable Profit start----------------
                        var yearsApart = new Date(new Date(SplitAlldates[0]) - new Date(SplitAlldates[i])).getFullYear() - 1970
                        var tprofit = 0;

                        if (SelectedScheme == 'D')// 3 is for debt tax
                        {
                            if (i == 0) {
                                htmlTable += '<td class="col-xs-1"></td>';
                            }
                            else {
                                if (Math.abs(yearsApart) > 3)//if > 4 then investment is long term if less then 4 investment is short term (4 is 3 years)
                                {
                                    // longterm
                                    var year = new Date(SplitAlldates[i]).getFullYear();
                                    var NextYear = year + 1 + ""
                                    var finanYear;
                                    var month = new Date(SplitAlldates[i]).getMonth();
                                    if (month <= 2) {
                                        var stringYear = year + "";
                                        finanYear = (year - 1) + '-' + stringYear[2] + stringYear[3]
                                    }
                                    else {
                                        finanYear = year + '-' + NextYear[2] + NextYear[3]
                                    }
                                    var CurrentCII = 0;
                                    for (var p = 0; p < CiiData.Table.length; p++) {
                                        if (CiiData.Table[p].FinancialYear == finanYear) {
                                            CurrentCII = CiiData.Table[p].CII;
                                            break;
                                        }
                                    }
                                    var InvDateCII = 0;
                                    var InvYear = $('#txtInvDate').val().split('-')[2];
                                    //var InvYear = $('#ddlInvYear :selected').text();
                                    var InvFinancialYear = '';
                                    if (parseInt($('#txtInvDate').val().split('-')[1]) > 3) {
                                        // if ((parseInt($('#ddlInvMonth').val()) + 1) > 3) {

                                        InvFinancialYear = InvYear + '-' + (parseInt(InvYear) + 1).toFixed(0)[2] + (parseInt(InvYear) + 1).toFixed(0)[3]
                                    }
                                    else {
                                        InvFinancialYear = (parseInt(InvYear) - 1) + '-' + (InvYear[2] + InvYear[3])
                                    }
                                    for (var t = 0; t < CiiData.Table.length; t++) {
                                        if (CiiData.Table[t].FinancialYear == finanYear) {
                                            InvDateCII = CiiData.Table[t].CII;
                                            break;
                                        }
                                    }

                                    tprofit = parseFloat($('#txtSWPAmount').val()) - (parseFloat(Math.abs(unitsTransacted)) * parseFloat(arrAllNavData[0].split('~')[3])) * parseFloat(CurrentCII) / parseFloat(InvDateCII);

                                    if (i != (arrAllNavData.length - 1)) {
                                        // htmlTable += '<td>' + Math.round(tprofit) + '</td>';
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(tprofit.toFixed(3)) + '</td>';
                                        //totalDebtProfit = totalDebtProfit + Math.round(tprofit);
                                        totalDebtProfit = totalDebtProfit + parseFloat(tprofit.toFixed(3));
                                        if (!isbalancedUnitNegative) {
                                            breakTotalDebtProfit = totalDebtProfit;
                                        }
                                    }
                                    else {
                                        if (i == (arrAllNavData.length - 1)) {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalDebtProfit)) + '</td>';
                                            //htmlTable += '<td>' + totalDebtProfit.toFixed(3) + '</td>';
                                        }
                                    }
                                    if (isbalancedUnitNegative) {
                                        htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalDebtProfit)) + '</td>';
                                    }
                                }
                                else {
                                    // shortterm
                                    tprofit = ((parseFloat(nav)) - (parseFloat(arrAllNavData[0].split('~')[3]))) * Math.abs(unitsTransacted);

                                    if (i != (arrAllNavData.length - 1)) {
                                        //htmlTable += '<td>' + Math.round(tprofit) + '</td>';
                                        //totalDebtProfit = totalDebtProfit + Math.round(tprofit);
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(tprofit.toFixed(3)) + '</td>';
                                        totalDebtProfit = totalDebtProfit + parseFloat(tprofit.toFixed(3));
                                        if (!isbalancedUnitNegative) {
                                            breakTotalDebtProfit = totalDebtProfit;
                                        }
                                    }
                                    else {
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalDebtProfit)) + '</td>';
                                        //htmlTable += '<td>' + totalDebtProfit.toFixed(3) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalDebtProfit)) + '</td>';
                                    }

                                }
                            }
                        }
                        else {
                            if (i == 0) {
                                htmlTable += '<td class="col-xs-1"></td>';
                            }
                            else {

                                if (Math.abs(yearsApart) <= 1) {
                                    if (i != (arrAllNavData.length - 1)) {
                                        //tprofit = (parseFloat($('#txtSWPAmount').val())) - (parseFloat(Math.round((Math.abs(costOfInvestment)))));
                                        tprofit = (parseFloat($('#txtSWPAmount').val())) - (parseFloat((Math.abs(costOfInvestment).toFixed(3))));
                                        //totalProfitForTavableProfit = totalProfit + tprofit;
                                        totalProfitForTavableProfit = totalProfitForTavableProfit + tprofit;
                                        //htmlTable += '<td>' + Math.round(tprofit) + '</td>';
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(tprofit.toFixed(3)) + '</td>';
                                        if (!isbalancedUnitNegative) {
                                            breakTotalProfitForTavableProfit = totalProfitForTavableProfit;
                                        }
                                    }
                                    else {
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalProfitForTavableProfit)) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalProfitForTavableProfit)) + '</td>';
                                    }
                                }
                                else {
                                    if (i != (arrAllNavData.length - 1)) {
                                        htmlTable += '<td class="col-xs-1">0</td>';
                                    }
                                    else {
                                        htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalProfitForTavableProfit)) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalProfitForTavableProfit)) + '</td>';
                                    }
                                }
                            }
                        }

                        //---------------------Taxable Profit End----------------

                        //---------------------Tax start----------------
                        var tax = 0;

                        if (SelectedScheme == 'D')// 3 is for debt tax
                        {
                            if (i == 0) {
                                htmlTable += '<td class="col-xs-1"></td>';
                            }
                            else {
                                if (Math.abs(yearsApart) > 3)//if > 4 then investment is long term if less then 4 investment is short term (4 is 3 years)
                                {
                                    //tax = ((Math.round(tprofit)) * 20.60) / 100;
                                    tax = (parseFloat((tprofit.toFixed(3))) * 20.60) / 100;

                                    if (i != (arrAllNavData.length - 1)) {
                                        //totalTax = totalTax + Math.round(tax);
                                        //htmlTable += '<td>' + Math.round(tax) + '</td>';
                                        totalTax = totalTax + parseFloat(tax.toFixed(3));
                                        if (parseFloat(tax) < 0) {
                                            //htmlTable += '<td>' + tax.toFixed(3) + '</td>';
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(tax.toFixed(3)) + '</td>';
                                        }
                                        if (!isbalancedUnitNegative) {
                                            breakTotalTax = totalTax;
                                        }
                                    }
                                    else {
                                        if (parseFloat(totalTax) < 0) {
                                            //htmlTable += '<td>' + Math.round(totalTax) + '</td>';
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalTax)) + '</td>';
                                        }
                                        //htmlTable += '<td>' + totalTax.toFixed(3) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        if (parseFloat(breakTotalTax) < 0) {
                                            //htmlTable += '<td>' + Math.round(totalTax) + '</td>';
                                            htmlTableforBreak += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalTax)) + '</td>';
                                        }
                                    }

                                }
                                else {
                                    //tax = (Math.round(tprofit) * 30.90) / 100;
                                    tax = (parseFloat(tprofit.toFixed(3)) * 30.90) / 100;
                                    if (i != (arrAllNavData.length - 1)) {
                                        //htmlTable += '<td>' + Math.round(tax) + '</td>';
                                        //totalTax = totalTax + Math.round(tax);
                                        if (parseFloat(tax) < 0) {
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(tax.toFixed(3)) + '</td>';
                                        }
                                        totalTax = totalTax + parseFloat(tax.toFixed(3));
                                        if (!isbalancedUnitNegative) {
                                            breakTotalTax = totalTax;
                                        }
                                    }
                                    else {
                                        if (parseFloat(totalTax) < 0) {
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalTax)) + '</td>';
                                        }
                                        //htmlTable += '<td>' + totalTax.toFixed(3) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        if (parseFloat(breakTotalTax) < 0) {
                                            htmlTableforBreak += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalTax)) + '</td>';
                                        }
                                    }

                                }
                            }
                        }
                        else {
                            if (i == 0) {
                                htmlTable += '<td class="col-xs-1"></td>';
                            }
                            else {
                                var yearsApart = new Date(new Date(SplitAlldates[0]) - new Date(SplitAlldates[i])).getFullYear() - 1970
                                if (Math.abs(yearsApart) <= 1) {

                                    //short term
                                    if (i != (arrAllNavData.length - 1)) {

                                        //var profit= (parseFloat($('#txtSWPAmount').val()))-(parseFloat(Math.round((Math.abs(costOfInvestment)))));
                                        tax = (tprofit * 15.45) / 100;


                                        //totalTax = totalTax + Math.round(tax);
                                        //htmlTable += '<td>' + Math.round(tax) + '</td>';
                                        totalTax = totalTax + parseFloat(tax.toFixed(3));
                                        if (!isbalancedUnitNegative) {
                                            breakTotalTax = totalTax;
                                        }
                                        if (parseFloat(tax) < 0) {
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(tax.toFixed(3)) + '</td>';
                                        }
                                    }
                                    else {
                                        if (parseFloat(totalTax) < 0) {
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalTax)) + '</td>';
                                        }
                                        //htmlTable += '<td>' + totalTax.toFixed(3) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        if (parseFloat(breakTotalTax) < 0) {
                                            htmlTableforBreak += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalTax)) + '</td>';
                                        }

                                    }
                                }
                                else {
                                    //long term tax 0
                                    if (i != (arrAllNavData.length - 1)) {
                                        htmlTable += '<td class="col-xs-1">0</td>';
                                    }
                                    else {
                                        if (parseFloat(totalTax) < 0) {
                                            htmlTable += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalTax)) + '</td>';
                                        }
                                        //htmlTable += '<td>' + totalTax.toFixed(3) + '</td>';
                                        //htmlTable += '<td>' + totalTax.toFixed(3) + '</td>';
                                    }
                                    if (isbalancedUnitNegative) {
                                        if (parseFloat(breakTotalTax) < 0) {
                                            htmlTableforBreak += '<td class="col-xs-1">0</td>';
                                        }
                                        else {
                                            htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalTax)) + '</td>';
                                        }

                                    }
                                }
                            }
                        }
                        //---------------------Tax End----------------

                        //---------------------Net Profit start----------------
                        var breakTotalNetProfit = 0;
                        if (i == 0) {
                            htmlTable += '<td class="col-xs-1"></td>';
                        }
                        else {
                            //adding for net profit

                            if (i != (arrAllNavData.length - 1)) {
                                //var netProfit = Math.round(tprofit) - Math.round(tax);
                                var netProfit = parseFloat(profit.toFixed(3)) - parseFloat(tax.toFixed(3));
                                totalNetProfit = totalNetProfit + netProfit;
                                if (!isbalancedUnitNegative) {
                                    breakTotalNetProfit = totalNetProfit;
                                }
                                //htmlTable += '<td>' + Math.round(netProfit) + '</td>';
                                htmlTable += '<td class="col-xs-1">' + formatNumber(netProfit.toFixed(3)) + '</td>';
                            }
                            else {
                                htmlTable += '<td class="col-xs-1">' + formatNumber(Math.round(totalNetProfit)) + '</td>';
                                //htmlTable += '<td>' + totalNetProfit.toFixed(3) + '</td>';
                            }
                            if (isbalancedUnitNegative) {
                                htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(Math.round(breakTotalNetProfit)) + '</td>';
                            }
                            //-----
                        }

                        //---------------------Net Profit End----------------

                        //---------------------Value of Balance Units start----------------
                        if (i == 0) {
                            htmlTable += '<td class="col-xs-1">' + formatNumber(initialInvestmentAmt) + '</td>';
                        }
                        else {
                            //adding for Value of Balance Units

                            var BalanceValue = nav * balanceUnit;
                            //htmlTable += '<td>' + Math.round(BalanceValue) + '</td>';
                            if (i == (arrAllNavData.length - 1)) {
                                // this box  is mainly used for cash flow calculation
                                //totalBalUnit = Math.round(BalanceValue);
                                var latestNav = GetLastestNavBySchemeCode();
                                BalanceValue = latestNav * balanceUnit;
                                totalBalUnit = parseFloat(BalanceValue.toFixed(3));
                                //htmlTable += '<td class="col-xs-1">' + formatNumber(totalBalUnit.toFixed(3)) + '</td>';
                            }
                            htmlTable += '<td class="col-xs-1">' + formatNumber(BalanceValue.toFixed(3)) + '</td>';
                            if (isbalancedUnitNegative) {
                                var latestNav = GetLastestNavBySchemeCode();
                                BalanceValue = latestNav * lastBreakBalUnit;
                                breakLastBalanceValue = BalanceValue;
                                htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(BalanceValue.toFixed(3)) + '</td>';
                            }

                        }


                        //---------------------Value of Balance Units End----------------

                        //---------------------Cash flow start----------------
                        if (i == 0) {
                            htmlTable += '<td class="col-xs-1">-' + formatNumber(initialInvestmentAmt) + '</td>';
                        }
                        else {

                            if (i != (arrAllNavData.length - 1)) {
                                htmlTable += '<td class="col-xs-1">' + formatNumber($('#txtSWPAmount').val()) + '</td>';
                            }
                            else {
                                //htmlTable += '<td>' + Math.round(totalBalUnit) + '</td>';
                                htmlTable += '<td class="col-xs-1">' + formatNumber(totalBalUnit.toFixed(3)) + '</td>';
                            }
                            if (isbalancedUnitNegative) {
                                htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(totalBalUnit.toFixed(3)) + '</td>';
                            }
                            //-----
                        }

                        //---------------------Cash Flow End----------------

                        //---------------------Net cash flow start----------------
                        if (i == 0) {
                            htmlTable += '<td class="col-xs-1">-' + formatNumber(initialInvestmentAmt) + '</td>';
                        }
                        else {

                            if (i != (arrAllNavData.length - 1)) {
                                var netCashFolw = (parseFloat($('#txtSWPAmount').val())) - tax;
                                //htmlTable += '<td>' + Math.round(netCashFolw) + '</td>';
                                htmlTable += '<td class="col-xs-1">' + formatNumber(netCashFolw.toFixed(3)) + '</td>';
                            }
                            else {
                                //htmlTable += '<td>' + Math.round(totalBalUnit) + '</td>';
                                htmlTable += '<td class="col-xs-1">' + formatNumber(totalBalUnit.toFixed(3)) + '</td>';
                            }
                            if (isbalancedUnitNegative) {
                                htmlTableforBreak += '<td class="col-xs-1">' + formatNumber(totalBalUnit.toFixed(3)) + '</td>';
                            }
                            //-----
                        }

                        //---------------------Net cash flow End----------------
                        htmlTable += '</tr>';
                        if (isbalancedUnitNegative) {
                            break;
                        }
                    }
                }
                else {
                    htmlTable += '<tr><td colspan="13">No SWP Data available to calculate.</td></tr>';
                }
                htmlTable += '</tbody></table>';
                $('#appendBreakage').html('');
                $('#appendBreakage').append(htmlTable);
                $("#table-container-our").show();
                if (isbalancedUnitNegative) {
                    $('.table-fixed tr:last').remove();
                    htmlTableforBreak += '</tr>'
                    $('.table-fixed').append(htmlTableforBreak);
                }
                //------------------------code for Summary table-----------------------------------------//
                var htmlForSummary = '';
                htmlForSummary += '<h3>Summary of result</h3><table width="100%" cellspacing="0" rules="all" class="swp-table" border="0" id="grdJobDetails1"> <tbody><tr><th scope="col">Invested Amount (Rs.)</th>';
                htmlForSummary += '<th scope="col" >Total Withdrawal Amount (Rs.)</th><th scope="col" >Principal Component (Rs.)</th><th scope="col" >Growth/Gains Component (Rs.)</th><th scope="col" >Tax (Rs.)</th>';
                htmlForSummary += '<th scope="col" >Units Outstanding</th><th scope="col" >Value of Investments after withdrawal(Rs.)</th></tr>';
                htmlForSummary += '<tr>';
                if (amountWithdrown != undefined && amountWithdrown != '') {
                    htmlForSummary += '<td>' + formatNumber($('#txtInitialInvAmount').val()) + '</td>';
                    if (isbalancedUnitNegative) {
                        var amtBreakWithdrown = formatNumber((amountWithdrown) - (parseFloat($('#txtSWPAmount').val())))
                        htmlForSummary += '<td>' + amtBreakWithdrown + '</td>';
                        var breakCostofInvTotal = formatNumber(Math.round(totalcostOfInvestment - breakCurrentCostInvestment));
                        htmlForSummary += '<td>' + breakCostofInvTotal + '</td>';
                        htmlForSummary += '<td>' + formatNumber(Math.round(breakProfit)) + '</td>';
                    }
                    else {
                        htmlForSummary += '<td>' + formatNumber(amountWithdrown) + '</td>';
                        htmlForSummary += '<td>' + formatNumber(Math.round(totalcostOfInvestment)) + '</td>';
                        htmlForSummary += '<td>' + formatNumber(Math.round(totalProfit)) + '</td>';
                    }

                    if (parseFloat(totalTax) < 0) {
                        htmlForSummary += '<td>0</td>';
                    }
                    else {
                        htmlForSummary += '<td>' + formatNumber(Math.round(totalTax)) + '</td>';
                    }
                    if (isbalancedUnitNegative) {
                        htmlForSummary += '<td>' + formatNumber(Math.round(lastBreakBalUnit)) + '</td>';
                        htmlForSummary += '<td>' + formatNumber(Math.round(breakLastBalanceValue)) + '</td>';
                    }
                    else {
                        htmlForSummary += '<td>' + formatNumber(Math.round(lastBalUnits)) + '</td>';
                        htmlForSummary += '<td>' + formatNumber(Math.round(totalBalUnit)) + '</td>';
                    }

                }
                else {
                    htmlForSummary += '<td colspan="8">No SWP Data available to calculate.</td>';
                }
                htmlForSummary += '</tr>';
                htmlForSummary += '</tbody></table>';
                $('#appendSummary').html('');
                $('#appendSummary').append(htmlForSummary);
                $('.disclaimer').show();
            } catch (e) {
                alert(e.message)
            }
        }
        function monthDiff(date1, date2) {
            var months;
            months = (date2.getFullYear() - date1.getFullYear()) * 12;
            months -= date1.getMonth();
            months += date2.getMonth();
            return months <= 0 ? 0 : months;
        }
function GetNavData(mode, dateForNextBusinessDay) {
    debugger;
            try {
                var MethodName = "usp_GetNavHistory";
                var parameters = [];
                var Data="";
                parameters[0] = "DateString=" + AllDates;
                parameters[1] = "SchemeCode=" + $('#ddlScheme :selected').val().split('`')[0];
                parameters[2] = "Mode=" + mode;
                if (mode != 'AllDates') {
                    parameters[3] = "DateForNextBusinessDay=" + dateForNextBusinessDay;
                }
                var pdata = { "MethodName": MethodName, "parameters": parameters };
                //$.ajax({
                //    type: "POST",
                //    contentType: "application/json; charset=utf-8",
                //    url: SBIMFJSONServiceUrl,
                //    data: JSON.stringify(pdata),
                //    dataType: "json",
                //    async: false,
                //    success: function (data, textStatus, jqXHR) {
                //        if (data != '') {
                //            var navData = JSON.parse(data);
                //            if (navData != null) {
                //                if (navData != '' && navData != null) {
                //                    Data = navData;
                //                }
                //            }
                //        }

                //    },
                //    error: function (jqXHR, textStatus, errorThrown) {
                //        loader.stop()
                //        //response([]);
                //    },
                //});

                var body = JSON.stringify(pdata);
                $.when(commonObj.apiHelper(SBIMFJSONServiceUrl, 'POST', body)).then(function (data) {
                    if (data != '') {
                        var navData = JSON.parse(data);
                        if (navData != null) {
                            if (navData != '' && navData != null) {
                                Data = navData;
                            }
                        }
                    }
                }, function (errorThrown) {
                    loader.stop()
                });


            } catch (e) {
                loader.stop()
                alert('An error occurred while fatching data, please try later.');
            }
            return Data;
        }
        function GetCIIData() {
            var MethodName = "Usp_GetCIIDataOnFinancialYear";
            var parameters = [];
            var Data;
            var pdata = { "MethodName": MethodName, "parameters": parameters };
            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: SBIMFJSONServiceUrl,
            //    data: JSON.stringify(pdata),
            //    dataType: "json",
            //    async: false,
            //    success: function (data, textStatus, jqXHR) {
            //        if (data != '') {
            //            var navData = JSON.parse(data);
            //            if (navData != null) {
            //                if (navData != '' && navData != null) {
            //                    Data = navData;
            //                }
            //            }
            //        }

            //    },
            //    error: function (jqXHR, textStatus, errorThrown) {
            //        //response([]);
            //    },
            //});

            var body = JSON.stringify(pdata);
            $.when(commonObj.apiHelper(SBIMFJSONServiceUrl, 'POST', body)).then(function (data) {
                if (data != '') {
                    var navData = JSON.parse(data);
                    if (navData != null) {
                        if (navData != '' && navData != null) {
                            Data = navData;
                        }
                    }
                }
            }, function (errorThrown) {
            });

            return Data;
        }
        function GetLastestNavBySchemeCode() {
            var MethodName = "Usp_GetNavBySchemeCode";
            var parameters = [];
            parameters[0] = "SchemeCode=" + $('#ddlScheme :selected').val().split('`')[0];;
            var Data;
            var pdata = { "MethodName": MethodName, "parameters": parameters };
            //$.ajax({
            //    type: "POST",
            //    contentType: "application/json; charset=utf-8",
            //    url: SBIMFJSONServiceUrl,
            //    data: JSON.stringify(pdata),
            //    dataType: "json",
            //    async: false,
            //    success: function (data, textStatus, jqXHR) {
            //        if (data != '') {
            //            var navData = JSON.parse(data);
            //            if (navData != null) {
            //                if (navData != '' && navData != null) {
            //                    Data = navData.Table[0].NAV;
            //                }
            //            }
            //        }

            //    },
            //    error: function (jqXHR, textStatus, errorThrown) {
            //        //response([]);
            //    },
            //});

            var body = JSON.stringify(pdata);
            $.when(commonObj.apiHelper(SBIMFJSONServiceUrl, 'POST', body)).then(function (data) {
                if (data != '') {
                    var navData = JSON.parse(data);
                    if (navData != null) {
                        if (navData != '' && navData != null) {
                            Data = navData.Table[0].NAV;
                        }
                    }
                }
            }, function (errorThrown) {
            });

            return Data;
        }
        function Validate() {
            ClearErrors();
            var isvalid = true;
            var ddlScheme = $('#ddlScheme :selected');
            var txtInitialInvAmount = $('#txtInitialInvAmount');
            var formatedDate = $('#txtInvDate').val().split('-');
            var txtInvDate = new Date(formatedDate[2] + '/' + formatedDate[1] + '/' + formatedDate[0]);
            var invstmntDay = $('#ddlInvDay :selected').text();
            var invstmntMonth = (parseInt($('#ddlInvMonth :selected').val())) + 1;
            //var invstmntYear = $('#ddlInvYear :selected').text();
            var txtSWPAmount = $('#txtSWPAmount');
            var swpStrtDay = $('#ddlSWPStartDay :selected').text();
            var swpStrtMonth = $('#ddlSWPStartMonth :selected').text();
            var swpStrtYear = $('#ddlSWPStartYear :selected').text();
            var strtDate = new Date(swpStrtYear + '/' + swpStrtMonth + '/' + swpStrtDay);
            var swpEndDay = $('#ddlSWPEndDay :selected').text();
            var swpEndMonth = $('#ddlSWPEndMonth :selected').text();
            var swpEndYear = $('#ddlSWPEndYear :selected').text();
            var endDate = new Date(swpEndYear + '/' + swpEndMonth + '/' + swpEndDay);
            var todayDate = new Date();
            if (ddlScheme.text() == '--Select--' || ddlScheme.text() == '') {
                isvalid = false;
                $('#lblSchemeError').css('display', 'block');
                $('#lblSchemeError').text('Please select scheme');
            }
            if (txtInitialInvAmount.val() == '') {
                isvalid = false;
                $('#lblInvAmtError').css('display', 'block');
                $('#lblInvAmtError').text('Please enter investment amount');
            }
            if ($('#txtInvDate').val() == '') {
                isvalid = false;
                $('#lblErrorInvDate').css('display', 'block');
                $('#lblErrorInvDate').text('Please enter investment date');
            }
            if (txtSWPAmount.val() == '') {
                isvalid = false;
                $('#lblSWPAmount').css('display', 'block');
                $('#lblSWPAmount').text('Please enter SWP amount');
            }
            if (swpStrtMonth == "Feb" && swpStrtDay == "30") {
                isvalid = false;
                $('#lblSWPStartDate').css('display', 'block');
                $('#lblSWPStartDate').text('invalid date for selected month');
            }
            if (txtSWPAmount.val() != '') {
                if (parseInt(txtSWPAmount.val()) < 500) {
                    isvalid = false;
                    $('#lblSWPAmount').css('display', 'block');
                    $('#lblSWPAmount').text('SWP amount cannot be less then 500');
                }
            }
            if ((txtInvDate > strtDate) || (txtInvDate > endDate)) {
                isvalid = false;
                $('#lblErrorInvDate').css('display', 'block');
                $('#lblErrorInvDate').text('Investment date cannot be greater then SWP start date and SWP end date.');
            }
            if (parseFloat(txtInitialInvAmount.val()) < parseFloat(txtSWPAmount.val())) {
                isvalid = false;
                $('#lblInvAmtError').css('display', 'block');
                $('#lblInvAmtError').text('Initial investment amount cannot be less then SWP amount.');
            }
            if (txtInvDate > todayDate) {
                isvalid = false;
                $('#lblErrorInvDate').css('display', 'block');
                $('#lblErrorInvDate').text('Investment date cannot be grater than today.');
            }
            if (strtDate > todayDate) {
                isvalid = false;
                $('#lblSWPStartDate').css('display', 'block');
                $('#lblSWPStartDate').text('SWP start date cannot be grater than today.');
            }
            if (endDate > todayDate) {
                isvalid = false;
                $('#lblSWPEndDate').css('display', 'block');
                $('#lblSWPEndDate').text('SWP end date cannot be grater than today.');
            }
            var stringEndDate = endDate.getDate() + '-' + (endDate.getMonth() + 1) + '-' + endDate.getFullYear();
            var stringTodayDate = todayDate.getDate() + '-' + (todayDate.getMonth() + 1) + '-' + todayDate.getFullYear();
            if (stringEndDate == stringTodayDate) {
                isvalid = false;
                $('#lblSWPEndDate').css('display', 'block');
                $('#lblSWPEndDate').text('SWP end date should be less than today.');
            }
            return isvalid;
}

        function ClearErrors() {
            $('#lblInvAmtError').css('display', 'none');
            $('#lblErrorInvDate').css('display', 'none');
            $('#lblSWPAmount').css('display', 'none');
            $('#lblErrorInvDate').css('display', 'none');
            $('#lblInvAmtError').css('display', 'none');
            $('#lblSchemeError').css('display', 'none');
            $('#lblSWPStartDate').css('display', 'none');
            $('#lblSWPEndDate').css('display', 'none');

            $('#lblInvAmtError').text('');
            $('#lblErrorInvDate').text('');
            $('#lblSWPAmount').text('');
            $('#lblErrorInvDate').text('');
            $('#lblInvAmtError').text('');
            $('#lblSchemeError').text('');
            $('#lblSWPStartDate').text('');
            $('#lblSWPEndDate').text('');
        }
        function fillYears() {
            var currntYear = new Date().getFullYear();
            for (i = currntYear; i > 1990; i--) {
                if (i == (currntYear - 1)) {
                    $('#ddlSWPStartYear').append('<option value="' + i + '"selected>' + i + '</option>');
                }
                else {
                    $('#ddlSWPStartYear').append('<option value="' + i + '">' + i + '</option>');
                }
                $('#ddlSWPEndYear').append('<option value="' + i + '">' + i + '</option>');

                //if (i == (currntYear - 2)) {
                //    $('#ddlInvYear').append('<option value="' + i + '"selected>' + i + '</option>');
                //}
                //else {
                //    $('#ddlInvYear').append('<option value="' + i + '">' + i + '</option>');
                //}
            }
        }
        function formatNumber(x) {
            x = x.toString();
            var afterPoint = '';
            if (x.indexOf('.') > 0) {
                var splitNumber = x.split('.')

                afterPoint = x.substring(x.indexOf('.'), x.length);
                x = splitNumber[0].toString();
            }
            else {
                x = x.toString();
            }
            var lastThree = x.substring(x.length - 3);
            var otherNumbers = x.substring(0, x.length - 3);
            if (otherNumbers != '') {
                if (otherNumbers != '-')
                    lastThree = ',' + lastThree;
            }
            var res = otherNumbers.replace(/\B(?=(\d{2})+(?!\d))/g, ",") + lastThree + afterPoint;

            return res;
        }
        $(function () {
            $('.placeholder-text').focus({
            });
        });
  