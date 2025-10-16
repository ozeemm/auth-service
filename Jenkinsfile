pipeline {
	agent any

	stages {
		stage('Checkout') {
			steps {
				checkout scm
			}
		}

		stage('Restore Dependencies') {
			steps {
				sh 'dotnet restore'
			}
		}

		stage('Build Dev'){
			when {
				expression { 
					return env.BRANCH_NAME == 'dev' || env.BRANCH_NAME.startsWith('feature/') 
				}
			}

			steps {
				sh 'dotnet build --configuration Debug --no-restore'
			}
		}

		stage('Build Release'){
			when {
				branch 'main'
			}

			steps {
				sh 'dotnet build --configuration Release --no-restore'
			}
		}

		stage('Unit tests'){
			steps {
				sh 'dotnet test --filter "Category=Unit" --no-build'
			}
		}

		stage('Integration tests'){
			steps {
				sh 'dotnet test --filter "Category=Integration" --no-build'
			}
		}

		stage('Publish Dev') {
			when {
				branch 'dev'
			}

			steps {
				sh 'dotnet publish --configuration Debug --self-contained false --output ./publish'
			}
		}

		stage('Publish Release') {
			when {
				branch 'main'
			}

			steps {
				sh 'dotnet publish --configuration Release --self-contained false --output ./publish'
			}
		}

		stage('Run Dev') {
			when {
				branch 'dev'
			}

			steps {
				sh '''
                    sudo systemctl restart AuthServiceApp_Dev
					sleep 5
					curl -f http://localhost:5127/api/Examples/health || exit 1
				'''
			}
		}

		stage('Run Release') {
			when {
				branch 'main'
			}

			steps {
				sh '''
                    sudo systemctl restart AuthServiceApp_Main
					sleep 5
					curl -f http://localhost:5128/api/Examples/health || exit 1
				'''
			}
		}
	}
}