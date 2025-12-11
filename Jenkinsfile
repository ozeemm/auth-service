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

		stage('Format') {
			steps {
				sh 'dotnet format --verify-no-changes --no-restore'
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

		stage('Docker Compose Tests Up') {
			steps {
				sh '''
					docker-compose -f docker-compose.tests.yml -p tests-db up -d
					sleep 5
				'''
			}
		}

		stage('Unit tests'){
			environment{
				ConnectionStrings__TestsDatabase = "Host=localhost;Port=5432;Database=devops_authservice_tests;Username=postgres;Password=postgres"
			}

			steps {
				sh 'dotnet test --filter "Category=Unit" --no-build'
			}
		}

		stage('Integration tests'){
			environment{
				ConnectionStrings__TestsDatabase = "Host=localhost;Port=5432;Database=devops_authservice_tests;Username=postgres;Password=postgres"
			}
			
			steps {
				sh 'dotnet test --filter "Category=Integration" --no-build'
			}
		}

		stage('Docker Compose Build Release') {
			when {
				branch 'main'
			}

			steps {
				sh '''
					docker-compose -f docker-compose.yml down
					docker-compose -f docker-compose.yml build --no-cache
				'''
			}
		}

		stage('Run Release') {
			when {
				branch 'main'
			}

			steps {
				sh '''
                    docker-compose -f docker-compose.yml up -d
					sleep 5
					curl -f http://localhost:5128/api/Examples/health || exit 1
				'''
			}
		}
	}

	post {
		always {
			sh 'docker-compose -f docker-compose.tests.yml -p tests-db down'
		}
	}
}