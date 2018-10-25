//google.charts.load("current", { packages: ["corechart"] });
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

    forgeryId = $("#forgeryToken").val();

            $.ajax({
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                url: SchemeChartDataUrl,
                data: {},
                headers: {
                    'VerificationToken': forgeryId
                },
                success: function (data) {
                    debugger;
                    var ran_key = CryptoJS.enc.Utf8.parse(key);
                    var iv = CryptoJS.enc.Utf8.parse(key);
                    var decrypteddata = CryptoJS.AES.decrypt(data.ResponseBody, ran_key, {
                        iv: iv,
                        padding: CryptoJS.pad.Pkcs7,
                        mode: CryptoJS.mode.CBC
                    });
                    decrypteddata = decrypteddata.toString(CryptoJS.enc.Utf8)
                    lst_Schemes = decrypteddata;
                    lst_Schemes = JSON.parse(lst_Schemes);
                    $('#dvSchChart').scrollView();
                    $("#lblFundCat").text(lst_Schemes[0].CategoryName);

                    var tot_ret = 0;

                    //var data = new google.visualization.DataTable();
                    //data.addColumn('string', 'Name');
                    //data.addColumn('number', 'currentvalue');
                    //// A column for custom tooltip content
                    //data.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });

                    var tot_curr = 0, tot_amt = 0, tot_gl = 0;

                    for (var i = 0; i < lst_Schemes.length; i++) {
                        tot_amt = tot_amt + (parseFloat((lst_Schemes[i].costvalue).replace(',', '')));
                        tot_ret = tot_ret + parseFloat(lst_Schemes[i].currentvalue);
                    }

                    //for (var i = 0; i < lst_Schemes.length; i++) {
                    //    var colorCode = "";
                    //    if (lst_Schemes[i].gain_loss == "Loss") {
                    //        colorCode = "#ed5249";
                    //    }
                    //    else if (lst_Schemes[i].gain_loss == "Gain") {
                    //        colorCode = "#4BB543";
                    //    }
                    //    else {
                    //        colorCode = "#0095da";
                    //    }

                    //    var percentage = ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / tot_amt).toFixed(2);

                    //    data.addRow([lst_Schemes[i].Name, parseFloat((lst_Schemes[i].costvalue).replace(',', '')), '<div style="white-space: nowrap; padding:5px;"><span style="color:#808082;"> Scheme Name : </span><span style="color:#0095da;">' + lst_Schemes[i].Name +
                    //        '</span><br><span style="color:#808082;">Amount Invested : </span><span style="color:#0095da;">&#x20b9;' + lst_Schemes[i].costvalue + ' (' + percentage + '%)' +
                    //        '</span><br><span style="color:#808082;">Current Value : </span><span style="color:#0095da;">&#x20b9;' + lst_Schemes[i].currentvalue +
                    //        '</span> <br><span style="color:#808082;">' + lst_Schemes[i].gain_loss + ': </span><span style="color:' + colorCode + ';">' + lst_Schemes[i].gain_loss_value + '%<span></div>']); //'Fund Name: ' + lst_Schemes[i].CategoryName + ' [' + lst_Schemes[i].gain_loss_tooltip + '(in %): ' + lst_Schemes[i].Scheme_Value + ']']); // '<div style="white-space: nowrap; padding:5px;"><b> Fund Name : ' + lst_Schemes[i].CategoryName + '</b><br>' + lst_Schemes[i].gain_loss_tooltip +' (in %) : ' + lst_Schemes[i].Scheme_Value + '</div>']);

                    //}

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
                            legend: {
                            },
                            credits: {
                                enabled: false
                            },
                            tooltip: {
                                backgroundColor: '#ffffff',
                                formatter: function () {
                                    var i = this.point.index;
                                    return '<div style="white-space: nowrap; padding:5px;"><span style="color:#808082;"> Scheme Name : </span><span style="color:#0095da;">' + lst_Schemes[i].Name +
                                        '</span><br><span style="color:#808082;">Amount Invested : </span><span style="color:#0095da;">' + lst_Schemes[i].costvalue + ' (' + ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / tot_amt).toFixed(0) + '%)' +
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
                                                loader.start();
                                                var i = event.point.index;
                                                var s_percentage = 0;
                                                //var Scheme = { Code: lst_Schemes[i].Code, Lname: lst_Schemes[i].Name, costvalue: lst_Schemes[i].costvalue, currentvalue: lst_Schemes[i].currentvalue };
                                                
                                                s_percentage = ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / tot_amt).toFixed(2);


                                                $.ajax({
                                                    type: 'POST',
                                                    dataType: 'json',
                                                    contentType: 'application/json',
                                                    url: GetSchemeD_ChartUrl,
                                                    headers: {
                                                        'VerificationToken': forgeryId
                                                    },
                                                    data: JSON.stringify({ sch_code: lst_Schemes[i].Code, sch_percentage: s_percentage })
                                                    , success: function (data) {
                                                        loader.stop();
                                                        window.location.href = DisplaySchemeD_ChartUrl
                                                    },
                                                    error: function () {
                                                        loader.stop();
                                                        alert("Error!.");
                                                    }
                                                });
                                                loader.stop();
                                            }
                                        }
                                    }
                                }
                            ],
                            "chart": {
                                "renderTo": "schChart"
                            },
                        });

                    if (tot_amt == 0) {
                        tot_gl = 0;
                        $("#no_data").css("display", "block");
                        $("#spanGL").css("color", "black");
                    }
                    else if (tot_amt > tot_ret) {
                        $("#no_data").css("display", "none");
                        $("#spanGL").css("color", "#ed5249");
                        tot_gl = parseFloat((tot_amt - tot_ret) * 100 / tot_amt).toFixed(2);
                    }
                    else {
                        $("#no_data").css("display", "none");
                        $("#spanGL").css("color", "#4BB543");
                        tot_gl = parseFloat((tot_ret - tot_amt) * 100 / tot_amt).toFixed(2);
                    }

                    $("#lblAmt").text(parseFloat(tot_amt).toFixed(2));
                    $("#lblTotRet").text(parseFloat(tot_ret).toFixed(2));
                    $("#lblGL").text(tot_gl);

                    //var options = {
                    //    //is3D: true,
                    //    pieHole: 0.4,
                    //    legend: { alignment: 'center', textStyle: { fontSize: 15, margin: 30, color: '#595959', bold: 'true' } },  //position: 'bottom',
                    //    tooltip: { isHtml: true },
                    //    width: '100%',
                    //    height: 300,
                    //    colors: ['#00a8e9', '#0187ce', '#04c14b', '#03ac13', '#b7b7b7', '#d7d7d7'],
                    //    chartArea: { width: '100%', top: '5%', left: '5%', bottom: '10%' },
                    //};
                    //var chart = new google.visualization.PieChart(document.getElementById('schChart'));
                    //google.visualization.events.addListener(chart, 'select', schChartSelectHandler);
                    //google.visualization.events.addListener(chart, 'onmouseover', onmouseover_handler);
                    //google.visualization.events.addListener(chart, 'onmouseout', onmouseout_handler);

                    //chart.draw(data, options);

                    //function onmouseover_handler() {
                    //    $('#schChart').css('cursor', 'pointer')
                    //}
                    //function onmouseout_handler() {
                    //    $('#schChart').css('cursor', 'default')
                    //}

                    //function schChartSelectHandler() {
                    //    loader.start();
                    //    var selectedItem = chart.getSelection()[0];
                    //    if (selectedItem) {
                    //        var i = selectedItem.row;
                    //        var s_percentage = 0;
                    //        //var Scheme = { Code: lst_Schemes[i].Code, Lname: lst_Schemes[i].Name, costvalue: lst_Schemes[i].costvalue, currentvalue: lst_Schemes[i].currentvalue };

                    //        var selectedItem = chart.getSelection()[0];
                    //        if (selectedItem) {
                    //            s_percentage = ((parseFloat((lst_Schemes[i].costvalue).replace(',', ''))) * 100 / tot_amt).toFixed(2);
                    //        }


                    //        $.ajax({
                    //            type: 'POST',
                    //            dataType: 'json',
                    //            contentType: 'application/json',
                    //            url: GetSchemeD_ChartUrl,
                    //            data: JSON.stringify({ sch_code: lst_Schemes[i].Code, sch_percentage: s_percentage })
                    //            , success: function (data) {
                    //                loader.stop();
                    //                window.location.href = DisplaySchemeD_ChartUrl
                    //            },
                    //            error: function () {
                    //                loader.stop();
                    //                alert("Error!.");
                    //            }
                    //        });
                    //    }
                    //}

                    loader.stop();
                },
                error: function (xhr, status, error) {
                    loader.stop();
                    alert("Error loading data for scheme chart!")
                }
            });

        });