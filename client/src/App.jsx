import AuthPage from './Components/AuthPage'
import UserPage from './Components/UserPage'
import EnvBlock from './Components/EnvBlock'
import { useState } from 'react'

function App() {

  const [userData, setUserData] = useState(null)

  function onLogin(accessToken, refreshToken, username){
    setUserData({
      'accessToken': accessToken,
      'refreshToken': refreshToken,
      'username': username
    })
  }

  function onLogout(){
    setUserData(null)
  }

  return (
    <>
      <div className="container">

        <div className="forms-section">
            <EnvBlock />
            { userData ? <UserPage userdata={userData} onLogout={onLogout}/> : <AuthPage onLogin={onLogin}/> }
        </div> 
    </div> 
    </>
  )
}

export default App
