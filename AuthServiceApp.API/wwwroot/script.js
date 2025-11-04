const loginForm = document.querySelector("#loginForm")
const registerForm = document.querySelector("#registerForm")

const authStatusContainer = document.querySelector("#authStatusContainer")
const getUsersContainer = document.querySelector("#getUsersContainer")
const loginContainer = document.querySelector("#loginContainer")
const registerContainer = document.querySelector("#registerContainer")

const getUsersButton = document.querySelector("#getUsersButton")
const logoutButton = document.querySelector("#logoutButton")
const usersList = document.querySelector("#usersList")
const authStatus = document.querySelector("#authStatus")

let accessToken = null
let refreshToken = null

function showLoginAndRegisterBlocks(){
    authStatusContainer.style.display = "none"
    getUsersContainer.style.display = "none"
    loginContainer.style.display = "block"
    registerContainer.style.display = "block"
    usersList.innerHTML = ""
}

function showAuthedBlocks(){
    authStatusContainer.style.display = "block"
    getUsersContainer.style.display = "block"
    loginContainer.style.display = "none"
    registerContainer.style.display = "none"
    usersList.innerHTML = ""
}

loginForm.addEventListener('submit', async (e) => {
    e.preventDefault()
    
    const username = e.target.loginUsername.value
    const password = e.target.loginPassword.value

    const response = await fetch('/api/Auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            'username': username,
            'password': password
        })
    })

    loginForm.reset()

    if(!response.ok){
        alert('Неверный логин или пароль')
        return
    }

    const data = await response.json()

    accessToken = data.accessToken
    refreshToken = data.refreshToken

    authStatus.innerText = `Авторизован(${username})`
    showAuthedBlocks()
})

registerForm.addEventListener('submit', async (e) => {
    e.preventDefault()

    const username = e.target.regUsername.value
    const password = e.target.regPassword.value

    const response = await fetch('/api/Auth/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            'username': username,
            'password': password
        })
    })

    if(!response.ok){
        alert('Пользователь существует')
        return
    }
})

getUsersButton.addEventListener('click', async (e) => {
    const response = await fetch('/api/User/', {
        headers:{
            'Authorization': `Bearer ${accessToken}`
        }
    })

    if(!response.ok){
        alert("Не удалось получить список пользователей")
        showLoginAndRegisterBlocks()
        return
    }

    const data = await response.json()

    usersList.innerHTML = ""

    data.forEach(user => {
        const userDiv = document.createElement("div")
        userDiv.innerHTML = `<div>${user.id} | ${user.username}</div>`
        usersList.appendChild(userDiv)
    });
})

logoutButton.addEventListener('click', async (e) => {
    const response = await fetch('/api/Auth/logout', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${accessToken}`
        }
    })

    if(response.ok){
        accessToken = null
        refreshToken = null
        showLoginAndRegisterBlocks()
    }
})