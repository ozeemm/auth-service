import { useState } from "react"

function UserPage({ userdata, onLogout }) {

    const [users, setUsers] = useState([])

    async function fetchUsers() {
        const response = await fetch('/api/User/', {
            headers:{
                'Authorization': `Bearer ${userdata.accessToken}`
            }
        })

        if(!response.ok){
            alert("Не удалось получить список пользователей")
            onLogout()
            return
        }

        const data = await response.json()

        setUsers(data)
    }

    async function logout() {
        const response = await fetch('/api/Auth/logout', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${userdata.accessToken}`
            }
        })

        if(!response.ok){
            alert('Не удалось выйти')
            return
        }
        onLogout()
    }

    return (
        <>
            <div id="authStatusContainer" className="form-container">
                <h2 id="authStatus" style={{ color: 'green' }}>Авторизован ({userdata.username})</h2>
                <button className="btn" id="logoutButton" style={{ backgroundColor: 'red' }} onClick={logout}>Выйти</button>
            </div>

            <div id="getUsersContainer" className="form-container">
                <h2>Список пользователей</h2>
                <button className="btn" id="getUsersButton" onClick={fetchUsers}>Получить</button>
                <div id="usersList">
                    {users.map((user) => (
                        <div key={user.id}> {user.id} | {user.username} </div>
                    ))}
                </div>
            </div>
        </>
    )
}

export default UserPage