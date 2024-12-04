// chart-bar-demo.js
let monthlyRequestsElement = document.getElementById("monthlyRequestsData");
let monthlyRequests = JSON.parse(monthlyRequestsElement.getAttribute("data-monthly-requests"));

let barLabels = Object.keys(monthlyRequests);
let barData = Object.values(monthlyRequests);

var barCtx = document.getElementById("myBarChart");
var myBarChart = new Chart(barCtx, {
    type: 'bar',
    data: {
        labels: barLabels,
        datasets: [{
            label: "Requests",
            backgroundColor: "#4e73df",
            hoverBackgroundColor: "#2e59d9",
            borderColor: "#4e73df",
            data: barData,
        }],
    },
    options: {
        maintainAspectRatio: false,
        layout: {
            padding: {
                left: 10,
                right: 25,
                top: 25,
                bottom: 0
            }
        },
        scales: {
            xAxes: [{
                gridLines: {
                    display: false,
                    drawBorder: false
                },
                ticks: {
                    maxTicksLimit: 6
                },
                maxBarThickness: 25,
            }],
            yAxes: [{
                ticks: {
                    maxTicksLimit: 5,
                    padding: 10,
                    callback: function(value) {
                        return number_format(value);
                    }
                },
                gridLines: {
                    color: "rgb(234, 236, 244)",
                    zeroLineColor: "rgb(234, 236, 244)",
                    drawBorder: false,
                    borderDash: [2],
                    zeroLineBorderDash: [2]
                }
            }],
        },
        legend: {
            display: false
        },
        tooltips: {
            titleMarginBottom: 10,
            titleFontColor: '#6e707e',
            titleFontSize: 14,
            backgroundColor: "rgb(255,255,255)",
            bodyFontColor: "#858796",
            borderColor: '#dddfeb',
            borderWidth: 1,
            xPadding: 15,
            yPadding: 15,
            displayColors: false,
            caretPadding: 10,
            callbacks: {
                label: function(tooltipItem) {
                    return 'Requests: ' + number_format(tooltipItem.yLabel);
                }
            }
        },
    }
});