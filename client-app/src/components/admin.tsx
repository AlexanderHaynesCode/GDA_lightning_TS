import { useState } from "react";
    
function Admin() {
    const [output, setOutput] = useState("");  

    async function DoTheDelete() {
        const response = await fetch('user/DeleteTheTestAutomationAccounts');
        const data = await response.json();
        
        if (data == 1) {
            setOutput("Test accounts are successfully deleted.");
        } else if (data == 2) {
            setOutput("No test account exists, nothing to delete.");
        } else {
            setOutput("Error deleting test accounts.");
        }
    }    

    return (
        <div>
            <h1>Admin</h1>

            <div className="row">
                <div className="col-md-6">
                    <input type="button" onClick={DoTheDelete} value='Delete the test automation accounts' />
                </div>
                <div className="col-md-6">
                    <p id="displayOutput">{output}</p>
                </div>
            </div>
        </div>
    )
}

export default Admin;