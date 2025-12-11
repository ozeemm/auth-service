function AuthPage({ onLogin }) {

  async function handleLogin(e){
    e.preventDefault()
    const formData = new FormData(e.target)
    
    const login = formData.get("login")
    const password = formData.get("password")

    const response = await fetch('/api/Auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            'username': login,
            'password': password
        })
    })

    e.target.reset()

    if(!response.ok){
        alert('Неверный логин или пароль')
        return
    }

    const data = await response.json()

    const accessToken = data.accessToken
    const refreshToken = data.refreshToken

    onLogin(accessToken, refreshToken, login)
  }

  async function handleRegister(e){
    e.preventDefault()
    const formData = new FormData(e.target)

    const username = formData.get('username')
    const password = formData.get('password')

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

    e.target.reset();

    if(!response.ok){
        alert('Пользователь существует')
        return
    }

    alert(`Пользователь ${username} успешно зарегистрирован`)
  }

    return (
        <>
            <div className="form-container" id="loginContainer">
                <h2>Авторизация</h2>
                <form id="loginForm" onSubmit={handleLogin}>
                    <div className="form-group">
                        <label htmlFor="loginUsername">Имя пользователя</label>
                        <input type="text" name="login" placeholder="Введите ваше имя пользователя" required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="loginPassword">Пароль</label>
                        <input type="password" name="password" placeholder="Введите ваш пароль" required />
                    </div>
                    <button type="submit" className="btn">Войти</button>
                </form>
            </div>

            <div className="form-container" id="registerContainer">
                <h2>Регистрация</h2>
                <form id="registerForm" onSubmit={handleRegister}>
                    <div className="form-group">
                        <label htmlFor="regUsername">Имя пользователя</label>
                        <input type="text" name="username" placeholder="Придумайте имя пользователя" required />
                    </div>
                    <div className="form-group">
                        <label htmlFor="regPassword">Пароль</label>
                        <input type="password" name="password" placeholder="Придумайте пароль" required />
                    </div>
                    <button type="submit" className="btn">Зарегистрироваться</button>
                </form>
            </div>
        </>
    )
}

export default AuthPage