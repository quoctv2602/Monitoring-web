onmessage = function (e) {
  if (e.data === "start") {
    // Do some computation

    done();
  }
};

function done() {
  // ===> This kills the worker.
  // Send back the results to the parent page
  // const notification = new Notification("Alert push from Service Workers");
}
