//google.charts.load("current", { packages: ["corechart"] });

//google.charts.setOnLoadCallback(genFunChart);
var loader = {};
var commonObj = new CommonJS();
loader = {
    start: function () {
        $('#SiteLoader').addClass('overlay-loader').append($('<div>').addClass('loader'));
    },
    stop: function () {
        $('#SiteLoader').removeClass('overlay-loader').empty();
    }
}

$(document).ready(function () {

    //$("#navbarNav a[href*='/FundCategory/Invest_summary']").addClass("active");

    var overallSummaryFlag = 1;
    var schemecodes = "";

    //setting from date to current fiscal year start date
    var today = new Date();
    var curMonth = today.getMonth();

    var fiscalDt = "";
    if (curMonth > 3) {
        fiscalDt = '01/Apr/' + today.getFullYear().toString();
    }
    else {
        fiscalDt = '01/Apr/' + (today.getFullYear() - 1).toString();
    }
    //end: setting from date to current fiscal year start date

    //$("#dtFrom").datepicker("setDate", fiscalDt);
    //$("#dtTo").datepicker("setDate", today);

    var month_names = ["Jan", "Feb", "Mar",
        "Apr", "May", "Jun",
        "Jul", "Aug", "Sep",
        "Oct", "Nov", "Dec"];
    var curr_dt = today.getDate() + '/' + month_names[today.getMonth()] + '/' + today.getFullYear();

    $("#lblDefaultDt").text(fiscalDt + ' to ' + curr_dt);

    //Date range picker
    var start = fiscalDt;
    var end = moment();


    $('#reportrange').daterangepicker({
        locale: {
            format: 'DD/MMM/YYYY'
        },
        startDate: start,
        endDate: end,
        maxDate: new Date(),
        ranges: {
            'Last 3 Months': [moment().subtract(3, 'month')],
            'Last 6 Months': [moment().subtract(6, 'month')],
            'Last 1 year': [moment().subtract(1, 'year')]
        }
    }, cb);
    cb(start, end);


    function cb(start, end) {

        if (overallSummaryFlag != 1) {

            $('#reportrange span').html(start.format('DD/MMM/YYYY') + ' - ' + end.format('DD/MMM/YYYY'));


            var dateFrom = start.format('DD/MMM/YYYY');
            var dateTo = end.format('DD/MMM/YYYY');

            //if (dateFrom > dateTo) {
            //    loader.stop();
            //    $("#lblErrMsg").css("display", "block");
            //    $("#lblErrMsg").text("From Date should not be greater than To Date.");
            //    //$.Toast("Error", "From Date should not be greater than To Date.", "error", { position_class: "toast-top-center", has_icon: true });
            //}
            //else {

            $("#lblErrMsg").css("display", "none");
            $("#lblErrMsg").text("");
            genFunChart(dateFrom, dateTo);

            //}
        }
        else {
            overallSummaryFlag = 0;
            $('#reportrange span').html(fiscalDt + ' - ' + curr_dt);
        }
    }



    //Genrate pie chart for current financial year
    genFunChart(fiscalDt, today);


    //Generate pie chart for selected dates
    //$("#btnGenChart").click(function () {
    //    loader.start();

    //    var dateFrom = $('#dtFrom').datepicker('getDate');
    //    var dateTo = $("#dtTo").datepicker('getDate');

    //    if (dateFrom > dateTo) {
    //        loader.stop();
    //        $("#lblErrMsg").css("display", "block");
    //        $("#lblErrMsg").text("From Date should not be greater than To Date.");
    //        //$.Toast("Error", "From Date should not be greater than To Date.", "error", { position_class: "toast-top-center", has_icon: true });
    //    }
    //    else {
    //        $("#lblErrMsg").css("display", "none");
    //        $("#lblErrMsg").text("");
    //        genFunChart(dateFrom, dateTo, 0);
    //    }
    //});

});



function genFunChart(dateFrom, dateTo) {
    debugger;
    loader.start();

    $("#dvFunChart").css("display", "block");
    $("#dvSchChart").css("display", "none");
    $("#dvTransactions").css("display", "none");


    var body = JSON.stringify({ dateFrom: dateFrom, dateTo: dateTo });
    $.when(commonObj.apiHelper('/FundCategory/GetChart', 'POST', body)).then(function (chartsdata) {
        debugger;
        if (typeof chartsdata.ErrMessage !== "undefined") {
            loader.stop();
            $("#lblErrMsg").css("display", "block");
            $("#lblErrMsg").text(chartsdata.ErrMessage);
            //$.Toast("Error", chartsdata.ErrMessage, "error", { position_class: "toast-top-center", has_icon: true });
        }
        else {
            $("#lblErrMsg").css("display", "none");
            $("#lblErrMsg").text("");

            var tot_curr = 0, tot_amt = 0, tot_gl = 0;
            for (var i = 0; i < chartsdata.length; i++) {
                tot_amt = tot_amt + parseFloat(chartsdata[i].Invest_Amt);
                tot_curr = tot_curr + parseFloat(chartsdata[i].Current_Value);
            }

            var Categories = [];
            var colorCodes = [];

            for (var i = 0; i < chartsdata.length; i++) {
                var point = [];
                //if (chartsdata[i].Invest_Amt != 0) {
                point.push(chartsdata[i].CategoryName);
                point.push(chartsdata[i].Invest_Amt);
                Categories.push(point);
                //}

                if (chartsdata[i].gain_loss_tooltip == "Loss") {
                    colorCodes.push("#ed5249");
                }
                else if (chartsdata[i].gain_loss_tooltip == "Gain") {
                    colorCodes.push("#4BB543");
                }
                else {
                    colorCodes.push("#0095da");
                }
            }

            chart = new Highcharts.Chart(
                {
                    plotOptions: {
                        pie: {
                            size: 210,
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: true,
                                color: '#ffffff',
                                formatter: function () {
                                    return Math.round(this.percentage, 2) + ' %';
                                },
                                distance: -30
                            },
                            showInLegend: true
                        }
                    },
                    legend: {
                    },
                    tooltip: {
                        backgroundColor: '#ffffff',
                        formatter: function () {
                            var i = this.point.index;
                            return '<div style="white-space: nowrap; padding:5px;"><span style="color:#c2c2c2;"> Category : </span><span style="color:#0095da;">' + chartsdata[i].CategoryName +
                                '</span><br><span style="color:#c2c2c2;">Amount Invested : </span><span style="color:#0095da;">' + chartsdata[i].Invest_Amt +
                                '</span><br><span style="color:#c2c2c2;">Current Value : </span><span style="color:#0095da;">' + chartsdata[i].Current_Value +
                                '</span> <br><span style="color:#c2c2c2;">' + chartsdata[i].gain_loss_tooltip + ' : </span><span style="color:' + colorCodes[i] + ';">' + chartsdata[i].Scheme_Value + '%<span></div>';
                        }
                    },
                    title: { text: '' },
                    series: [
                        {
                            "data": Categories.filter(function (d) { return d[1] > 0 }),
                            type: 'pie',
                            animation: false,
                            innerSize: '30%',
                            slicedOffset: 0,
                            colors: ['#00a8e9', '#0187ce', '#04c14b', '#03ac13', '#b7b7b7', '#d7d7d7'],
                            point: {
                                events: {
                                    click: function (event) {
                                        var index = event.point.index;
                                        var lst_Schemes = chartsdata[index].lst_Scheme;
                                        genSchemeChart(lst_Schemes);
                                    }
                                }
                            }
                        }
                    ],
                    "chart": {
                        "renderTo": "funChart"
                    },
                });

            //Calculate absolute gain/loss
            if (tot_amt == 0) {
                tot_gl = 0;
                $("#no_data").css("display", "block");
                $("#spanGL").css("color", "black");
            }
            else if (tot_amt > tot_curr) {
                $("#no_data").css("display", "none");
                $("#spanGL").css("color", "#ed5249");
                tot_gl = parseFloat((tot_amt - tot_curr) * 100 / tot_amt).toFixed(2);
            }
            else {
                $("#no_data").css("display", "none");
                $("#spanGL").css("color", "#4BB543");
                tot_gl = parseFloat((tot_curr - tot_amt) * 100 / tot_amt).toFixed(2);
            }

            $("#lblAmt").text(parseFloat(tot_amt).toFixed(2));                                //Total gain/loss
            $("#lblGL").text(tot_gl);                                //Total gain/loss
            $("#lblCurr").text(parseFloat(tot_curr).toFixed(2));     //Total Absolute Returns
            //end


            loader.stop();
        }


    }, function (errorThrown) {
        loader.stop();
        alert("Error loading data for fund category chart! Please try again.");
    });



    //$.ajax({
    //    type: 'POST',
    //    dataType: 'json',
    //    contentType: 'application/json',
    //    url: '/FundCategory/GetChart',
    //    data: JSON.stringify({ dateFrom: dateFrom, dateTo: dateTo }),
    //    success: function (chartsdata) {

    //        // Callback that creates and populates a data table,
    //        // instantiates the pie chart, passes in the data and
    //        // draws it.  

    //        //chartsdata = data.lstFundCat;

    //        //if (overallSummaryFlag == 1) {

    //        //    var tot_Inv_Amt = data.tot_Inv_Amt, tot_Portfolio_Val = data.tot_Portfolio_Val, tot_GL = data.tot_GL;

    //        //    $("#lblOAmt").text(tot_Inv_Amt);
    //        //    $("#lblOCurr").text(tot_Portfolio_Val);
    //        //    $("#lblOGL").text(tot_GL);

    //        //    if(tot_Inv_Amt > tot_Portfolio_Val)
    //        //    {
    //        //        $("#spanOGL").css("color", "#ed5249");
    //        //    }
    //        //    else if (tot_Inv_Amt < tot_Portfolio_Val) {
    //        //        $("#spanOGL").css("color", "#4BB543");
    //        //    }
    //        //    else {
    //        //        $("#spanOGL").css("color", "#0095da");
    //        //    }
    //        //}

    //        if (typeof chartsdata.ErrMessage !== "undefined") {
    //            loader.stop();
    //            $("#lblErrMsg").css("display", "block");
    //            $("#lblErrMsg").text(chartsdata.ErrMessage);
    //            //$.Toast("Error", chartsdata.ErrMessage, "error", { position_class: "toast-top-center", has_icon: true });
    //        }
    //        else {
    //            $("#lblErrMsg").css("display", "none");
    //            $("#lblErrMsg").text("");

    //            var tot_curr = 0, tot_amt = 0, tot_gl = 0;
    //            for (var i = 0; i < chartsdata.length; i++) {
    //                tot_amt = tot_amt + parseFloat(chartsdata[i].Invest_Amt);
    //                tot_curr = tot_curr + parseFloat(chartsdata[i].Current_Value);
    //            }

    //            var Categories = [];
    //            var colorCodes = [];

    //            for (var i = 0; i < chartsdata.length; i++) {
    //                var point = [];
    //                //if (chartsdata[i].Invest_Amt != 0) {
    //                point.push(chartsdata[i].CategoryName);
    //                point.push(chartsdata[i].Invest_Amt);
    //                Categories.push(point);
    //                //}

    //                if (chartsdata[i].gain_loss_tooltip == "Loss") {
    //                    colorCodes.push("#ed5249");
    //                }
    //                else if (chartsdata[i].gain_loss_tooltip == "Gain") {
    //                    colorCodes.push("#4BB543");
    //                }
    //                else {
    //                    colorCodes.push("#0095da");
    //                }
    //            }

    //            chart = new Highcharts.Chart(
    //                {
    //                    plotOptions: {
    //                        pie: {
    //                            size: 210,
    //                            allowPointSelect: true,
    //                            cursor: 'pointer',
    //                            dataLabels: {
    //                                enabled: true,
    //                                color: '#ffffff',
    //                                formatter: function () {
    //                                    return Math.round(this.percentage, 2) + ' %';
    //                                },
    //                                distance: -30
    //                            },
    //                            showInLegend: true
    //                        }
    //                    },
    //                    legend: {
    //                    },
    //                    tooltip: {
    //                        backgroundColor: '#ffffff',
    //                        formatter: function () {
    //                            var i = this.point.index;
    //                            return '<div style="white-space: nowrap; padding:5px;"><span style="color:#c2c2c2;"> Category : </span><span style="color:#0095da;">' + chartsdata[i].CategoryName +
    //                                '</span><br><span style="color:#c2c2c2;">Amount Invested : </span><span style="color:#0095da;">' + chartsdata[i].Invest_Amt +
    //                                '</span><br><span style="color:#c2c2c2;">Current Value : </span><span style="color:#0095da;">' + chartsdata[i].Current_Value +
    //                                '</span> <br><span style="color:#c2c2c2;">' + chartsdata[i].gain_loss_tooltip + ' : </span><span style="color:' + colorCodes[i] + ';">' + chartsdata[i].Scheme_Value + '%<span></div>';
    //                        }
    //                    },
    //                    title: { text: '' },
    //                    series: [
    //                        {
    //                            "data": Categories.filter(function (d) { return d[1] > 0 }),
    //                            type: 'pie',
    //                            animation: false,
    //                            innerSize: '30%',
    //                            slicedOffset: 0,
    //                            colors: ['#00a8e9', '#0187ce', '#04c14b', '#03ac13', '#b7b7b7', '#d7d7d7'],
    //                            point: {
    //                                events: {
    //                                    click: function (event) {
    //                                        var index = event.point.index;
    //                                        var lst_Schemes = chartsdata[index].lst_Scheme;
    //                                        genSchemeChart(lst_Schemes);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    ],
    //                    "chart": {
    //                        "renderTo": "funChart"
    //                    },
    //                });

    //            //Calculate absolute gain/loss
    //            if (tot_amt == 0) {
    //                tot_gl = 0;
    //                $("#no_data").css("display", "block");
    //                $("#spanGL").css("color", "black");
    //            }
    //            else if (tot_amt > tot_curr) {
    //                $("#no_data").css("display", "none");
    //                $("#spanGL").css("color", "#ed5249");
    //                tot_gl = parseFloat((tot_amt - tot_curr) * 100 / tot_amt).toFixed(2);
    //            }
    //            else {
    //                $("#no_data").css("display", "none");
    //                $("#spanGL").css("color", "#4BB543");
    //                tot_gl = parseFloat((tot_curr - tot_amt) * 100 / tot_amt).toFixed(2);
    //            }

    //            $("#lblAmt").text(parseFloat(tot_amt).toFixed(2));                                //Total gain/loss
    //            $("#lblGL").text(tot_gl);                                //Total gain/loss
    //            $("#lblCurr").text(parseFloat(tot_curr).toFixed(2));     //Total Absolute Returns
    //            //end
                

    //            loader.stop();
    //        }


    //    },
    //    error: function (xhr, status, error) {
    //        loader.stop();
    //        alert("Error loading data for fund category chart! Please try again.");
    //    }

    //});
}

$.fn.scrollView = function () {
    return this.each(function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top
        }, 1000);
    });
}

function genSchemeChart(lst_Schemes) {
    $("#dvFunChart").css("display", "none");
    $("#dvSchChart").css("display", "block");
    $("#dvTransactions").css("display", "none");


    $("#lblFundCat").text(lst_Schemes[0].CategoryName);

    var sch_tot_ret = 0;
    

    var sch_tot_curr = 0, sch_tot_amt = 0, sch_tot_gl = 0;

    for (var i = 0; i < lst_Schemes.length; i++) {
        sch_tot_amt = sch_tot_amt + (parseFloat((lst_Schemes[i].costvalue).replace(',', '')));
        sch_tot_ret = sch_tot_ret + parseFloat(lst_Schemes[i].currentvalue);
    }

    var Schemes = [];
    var sch_colorCode = [];

    for (var i = 0; i < lst_Schemes.length; i++) {
        var point = [];
        //if (parseFloat((lst_Schemes[i].costvalue).replace(',', '')) != 0) {
        point.push(lst_Schemes[i].Name);
        point.push(parseFloat((lst_Schemes[i].costvalue).replace(',', '')));
        Schemes.push(point);

        if (lst_Schemes[i].gain_loss == "Loss") {
            sch_colorCode.push("#ed5249");
        }
        else if (lst_Schemes[i].gain_loss == "Gain") {
            sch_colorCode.push("#4BB543");
        }
        else {
            sch_colorCode.push("#0095da");
        }
        //}
    }

    chart = new Highcharts.Chart(
        {
            plotOptions: {
                pie: {
                    size: '100%',
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        color: '#ffffff',
                        formatter: function () {
                            return Math.round(this.percentage, 2) + ' %';
                        },
                        distance: -20
                    },
                    showInLegend: true
                }
            },
            legend: {
            },
            tooltip: {
                backgroundColor: '#ffffff',
                formatter: function () {
                    var i = this.point.index;
                    return '<div style="white-space: nowrap; padding:5px;"><span style="color:#808082;"> Scheme Name : </span><span style="color:#0095da;">' + lst_Schemes[i].Name +
                        '</span><br><span style="color:#808082;">Amount Invested : </span><span style="color:#0095da;">' + lst_Schemes[i].costvalue + ' (' + ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / sch_tot_amt).toFixed(0) + '%)' +
                        '</span><br><span style="color:#808082;">Current Value : </span><span style="color:#0095da;">' + lst_Schemes[i].currentvalue +
                        '</span> <br><span style="color:#808082;">' + lst_Schemes[i].gain_loss + ': </span><span style="color:' + sch_colorCode[i] + ';">' + lst_Schemes[i].gain_loss_value + '%<span></div>';
                }
            },
            title: { text: '' },
            series: [
                {
                    "data": Schemes.filter(function (d) { return d[1] > 0 }),
                    type: 'pie',
                    animation: false,
                    innerSize: '30%',
                    slicedOffset: 0,
                    colors: ['#00a8e9', '#0187ce', '#04c14b', '#03ac13', '#b7b7b7', '#c7c7c7'],
                    point: {
                        events: {
                            click: function (event) {
                                var i = event.point.index;
                                var s_percentage = 0;
                                s_percentage = ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / sch_tot_amt).toFixed(2);
                                $("#sp_schPercentage").text(s_percentage);

                                genSchTrans(lst_Schemes, i);
                            }
                        }
                    }
                }
            ],
            "chart": {
                "renderTo": "schChart"
            },
        });
    

    if (sch_tot_amt == 0) {
        sch_tot_gl = 0;
        $("#no_data_sch").css("display", "block");
        $("#spanGL_sch").css("color", "black");
    }
    else if (sch_tot_amt > sch_tot_ret) {
        $("#no_data_sch").css("display", "none");
        $("#spanGL_sch").css("color", "#ed5249");
        sch_tot_gl = parseFloat((sch_tot_amt - sch_tot_ret) * 100 / sch_tot_amt).toFixed(2);
    }
    else {
        $("#no_data_sch").css("display", "none");
        $("#spanGL_sch").css("color", "#4BB543");
        sch_tot_gl = parseFloat((sch_tot_ret - sch_tot_amt) * 100 / sch_tot_amt).toFixed(2);
    }

    $("#lblAmt_sch").text(parseFloat(sch_tot_amt).toFixed(2));
    $("#lblTotRet_sch").text(parseFloat(sch_tot_ret).toFixed(2));
    $("#lblGL_sch").text(sch_tot_gl);

  

    //$('#dvSchChart').scrollView();
    loader.stop();
}

function genSchTrans(sch_trans, i) {

    $("#sp_schName").text(sch_trans[i].Name);
    $("#sp_schCostValue").text(sch_trans[i].costvalue);
    $("#sp_schCurrValue").text(sch_trans[i].currentvalue);
    $("#sp_schGL").text(sch_trans[i].gain_loss);
    $("#sp_schGLVal").text(sch_trans[i].gain_loss_value);

    if (sch_trans[i].gain_loss == "Gain") {
        $(".GL_Col").css("color", "#4BB543");
    }
    else if (sch_trans[i].gain_loss == "Loss") {
        $(".GL_Col").css("color", "#ed5249");
    }
    else {
        $(".GL_Col").css("color", "#0095da");
    }

    var lst_Trans = sch_trans[i].schemetransact;
    $('#tblTrans').DataTable({
        responsive: true,
        data: lst_Trans,
        "ordering": true,
        columns: [
            { "data": "Date", title: "Date" },
            { "data": "Type", title: "Transaction Type" },
            { "data": "Amount", title: "Amount" },
            { "data": "Nav", title: "NAV in INR (Rs.)" },
            { "data": "Price", title: "Price in INR (Rs.)" },
            { "data": "N_units", title: "Number of Units" },
            { "data": "B_units", title: "Balance Unit" }
        ]
    });

    $("#dvFunChart").css("display", "none");
    $("#dvSchChart").css("display", "none");
    $("#dvTransactions").css("display", "block");
    //$('#dvTransactions').scrollView();

    loader.stop();
}

function gotoFunChart() {
    $("#dvFunChart").css("display", "block");
    $("#dvSchChart").css("display", "none");
    $("#dvTransactions").css("display", "none");
    $('#dvFunChart').scrollView();
}
function gotoDFunChart() {
    $("#tblTrans").dataTable().fnDestroy();
    gotoFunChart();
}
function gotoSchChart() {
    $("#tblTrans").dataTable().fnDestroy();

    $("#dvFunChart").css("display", "none");
    $("#dvSchChart").css("display", "block");
    $("#dvTransactions").css("display", "none");
    $('#dvSchChart').scrollView();
}
