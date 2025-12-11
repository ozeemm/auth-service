import { useEffect, useState } from 'react'

function EnvBlock(){
  const [isDevelopment, setIsDevelopment] = useState(null)

  useEffect(() => {
    async function fetchIsDevelopment(){
      const response = await fetch('/api/environment/isDevelopment')
      const data = await response.json()

      setIsDevelopment(data.isDevelopment)
    }

    fetchIsDevelopment()
  }, [])

    return (
        <div className="form-container">
            <h2 id="environmentHeader">Environment: { (isDevelopment != null ? (isDevelopment ? 'Development' : 'Release') : '') }</h2>
            <a href="/swagger"><button className="btn swagger" disabled={!isDevelopment}>Swagger</button></a>
        </div>
    )
}

export default EnvBlock