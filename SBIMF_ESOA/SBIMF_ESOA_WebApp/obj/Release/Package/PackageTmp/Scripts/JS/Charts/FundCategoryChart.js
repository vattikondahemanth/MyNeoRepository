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

$.fn.scrollView = function () {
    return this.each(function () {
        $('html, body').animate({
            scrollTop: $(this).offset().top
        }, 1000);
    });
}

$(document).ready(function () {

    //$("#navbarNav a[href*='/FundCategory/Invest_summary']").addClass("active");

    forgeryId = $("#forgeryToken").val();

    var overallSummaryFlag = 1;  
    var schemecodes = "";

    //$('input[type=datetime]').datepicker({
    //    dateFormat: "dd/M/yy",
    //    changeMonth: true,
    //    changeYear: true,
    //    yearRange: "-60:+0",
    //    maxDate: new Date()
    //});    

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


});

        

function genFunChart(dateFrom, dateTo) {
    $('#dvFunChart').scrollView();

    loader.start();

    var body = JSON.stringify({ dateFrom: dateFrom, dateTo: dateTo });

    $.when(commonObj.apiHelper(FundChartUrl, 'POST', body)).then(function (chartsdata) {
        debugger;
        var chartsdata = JSON.parse(chartsdata);
        if (typeof chartsdata.ErrMessage !== "undefined") {
            loader.stop();

            if (chartsdata.ErrMessage == "No Data") {
                $("#no_data").css("display", "block");
                $("#spanGL").css("color", "black");
                $("#dv_chart").css("display", "none");
            }
            else {
                $("#lblErrMsg").css("display", "block");
                $("#lblErrMsg").text(chartsdata.ErrMessage);
            }
        }
        else {
            $("#dv_chart").css("display", "block");
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
                            size: '80%',
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
                    credits: {
                        enabled: false
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
                            credits: {
                                enabled: false
                            },
                            point: {
                                events: {
                                    click: function (event) {
                                        loader.start();
                                        var index = event.point.index;
                                        var lst_Schemes = chartsdata[index].lst_Scheme;
                                        var body = JSON.stringify({ lSchemes: lst_Schemes, dtFrom: dateFrom, dtTo: dateTo });
                                        $.when(commonObj.apiHelper(GetSchemeChartUrl, 'POST', body)).then(function (resp) {
                                            loader.stop();
                                            window.location.href = SchemeChartUrl
                                        }, function (errorThrown) {
                                            loader.stop();
                                            alert("Error!.");
                                            });
                                        loader.stop();
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

            //function selectHandler() {
            //    loader.start();
            //    var selectedItem = chart.getSelection()[0];
            //    if (selectedItem) {
            //        var index = selectedItem.row;
            //        //var lst_Schemes = data.getValue(selectedItem.row, 3);
            //        var lst_Schemes = chartsdata[index].lst_Scheme; 
            //        $.ajax({
            //            type: 'POST',
            //            dataType: 'json',
            //            contentType: 'application/json',
            //            url: GetSchemeChartUrl,
            //            data: JSON.stringify({ lSchemes: lst_Schemes, dtFrom: dateFrom, dtTo: dateTo })
            //            , success: function (data) {
            //                loader.stop();
            //                window.location.href = SchemeChartUrl
            //            },
            //            error: function () {
            //                loader.stop();
            //                alert("Error!.");
            //            }
            //        });

            //    }
            //}

            loader.stop();
        }


    }, function (errorThrown) {
        loader.stop();
        alert("Error loading data for fund category chart! Please try again.");
    });

}

//function genSchChart(lst_Schemes) {
//    var getHeaderHeight = jQuery("#divChart").height();
//    $('html, body').stop().animate({
//        'scrollTop': $("#divSchChart").offset().top //- getHeaderHeight - (getHeaderHeight * !$("#divChart").hasClass("navbar-fixed-top"))
//    }, 'fast', 'swing');

//    //$('html, body').animate({
//    //    scrollTop: $("#divSchChart").offset().top
//    //}, 2000);

//    document.getElementById("divSchChart").style.display = "block";
//    //document.getElementById("lblFundCat").innerHTML = lst_Schemes[0].CategoryName;

//    var Schemes = new Array();
//    Schemes[0] = ['Lname', 'currentvalue', 'Code', 'schemetransact'];
//    for (var i = 0; i < lst_Schemes.length; i++) {
//        Schemes[i + 1] = [lst_Schemes[i].Lname, parseFloat(lst_Schemes[i].currentvalue), lst_Schemes[i].Code, lst_Schemes[i].schemetransact];
//    }

//    var data = google.visualization.arrayToDataTable(Schemes, false);

//    var options = {
//        title: 'Scheme distribution',
//        is3D: true,
//        //pieHole: 0.4,
//    };

//    var chart = new google.visualization.PieChart(document.getElementById('schChart'));

//    google.visualization.events.addListener(chart, 'select', schChartSelectHandler);
//    chart.draw(data, options);

//    function schChartSelectHandler() {
//        var selectedItem = chart.getSelection()[0];
//        debugger;
//        if (selectedItem) {
//            var lst_SchTransactions = data.getValue(selectedItem.row, 3);
//            alert(lst_SchTransactions);
//            //alert(lst_SchTransactions[0].Code);
         

//            var body = JSON.stringify({ schTransactions: lst_SchTransactions });

//            $.when(commonObj.apiHelper('/SchTransaction/GetTransactionsfromChart', 'POST', body)).then(function (chartsdata) {
//                debugger;
//                window.location.href = "/SchTransaction/DisplayTransactionsfromChart";

//            }, function (errorThrown) {
//                alert("Error!.");
//            });

//        }
//    }
//}