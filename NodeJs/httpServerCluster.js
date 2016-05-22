'use strict'

const http = require('http')
const fs = require('fs')
const cluster = require('cluster')

const processorNumber = 6
let [port, fileNumber, directory] = [9669, 800, "E:\\RemotedProjects\\CourseWork\\data"]

const quickSort = (array, left, right) => {
    let temp;
    let x = array[left + (right - left) / 2];
    let i = left;
    let j = right;

    while (i <= j) {
        while (array[i] < x) {
            i++
        }
        while (array[j] > x) {
            j--
        }
        if (i <= j) {
            temp = array[i];
            array[i] = array[j];
            array[j] = temp;
            i++;
            j--;
        }
    }
    if (i < right)
        quickSort(array, i, right);

    if (left < j)
        quickSort(array, left, j);
}

const createServer = () => {
    http.createServer((request, response) => {
        const file = parseInt(request.url.substring(1))
        let fileName = `000${file % fileNumber}`.slice(-3)
        fileName = `file${fileName}.txt`

        fs.readFile(`${directory}\\${fileName}`, 'ascii', (err, data) => {
            if (err) {
                response.writeHead(400, {'Content-Type': 'text/plain'})
                response.end()
            }
            else {
                const array = data.toString().split("\r\n")
                const time = process.hrtime()
                quickSort(array, 0, array.length - 1)
                const [begin,end] = process.hrtime(time)
                const interval = (begin * 1e9 + end) / 1000000

                response.writeHead(200, {'Content-Type': 'text/plain'})
                response.end(`file: ${fileName}, length: ${array.length}, time: ${interval}`)

            }
        })
    }).listen(port, '127.0.0.1')
    console.log(`Server running at http://127.0.0.1:${port}/`)
}


// if (cluster.isMaster) {
//     for (let i = 0; i < processorNumber; i++) {
//         cluster.fork()
//     }

//     cluster.on('exit', (worker, code, signal) => {
//         console.log('worker ' + worker.process.pid + ' died')
//     })
// }
// else {
createServer()
//}

    