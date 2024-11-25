// Access the data from the div
const problemData = document.getElementById("problemData");

const openProblems = problemData.getAttribute("data-open-problems");
const pendingProblems = problemData.getAttribute("data-pending-problems");
const closedProblems = problemData.getAttribute("data-closed-problems");


// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
let crmCtx = document.getElementById("myPieChart");
let myPieChart = new Chart(crmCtx, {
    type: 'doughnut',
    data: {
        labels: ["Open", "Pending", "Close"],
        datasets: [{
            data: [openProblems, pendingProblems, closedProblems],
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
