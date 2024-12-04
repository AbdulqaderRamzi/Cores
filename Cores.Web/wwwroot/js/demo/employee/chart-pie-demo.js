// chart-pie-demo.js
const requestData = document.getElementById("requestData");

const openRequests = requestData.getAttribute("data-open-requests");
const pendingRequests = requestData.getAttribute("data-pending-requests");
const closedRequests = requestData.getAttribute("data-closed-requests");

let pieCtx = document.getElementById("myPieChart");
let myPieChart = new Chart(pieCtx, {
    type: 'doughnut',
    data: {
        labels: ["Open", "Pending", "Closed"],
        datasets: [{
            data: [openRequests, pendingRequests, closedRequests],
            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
            hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
            hoverBorderColor: "rgba(234, 236, 244, 1)",
        }],
    },
    options: {
        maintainAspectRatio: false,
        tooltips: {
            backgroundColor: "rgb(255,255,255)",
            bodyFontColor: "#858796",
            borderColor: '#dddfeb',
            borderWidth: 1,
            xPadding: 15,
            yPadding: 15,
            displayColors: false,
            caretPadding: 10,
        },
        legend: {
            display: false
        },
        cutoutPercentage: 80,
    },
});