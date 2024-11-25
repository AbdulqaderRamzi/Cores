// Access the data from the div
const todoData = document.getElementById("todoData");

const inProgressTodos = todoData.getAttribute("data-inProgress-todo");
const completedTodos = todoData.getAttribute("data-completed-todo");
const failedTodos = todoData.getAttribute("data-failed-todo");

console.log("In Progress:", inProgressTodos);
console.log("Completed:", completedTodos);
console.log("Failed:", failedTodos);

// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

// Pie Chart Example
let hrCtx = document.getElementById("myPieChart");
let myPieChart = new Chart(hrCtx, {
    type: 'doughnut',
    data: {
        labels: ["In-Progress", "Completed", "Failed"],
        datasets: [{
            data: [inProgressTodos, completedTodos, failedTodos],
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
