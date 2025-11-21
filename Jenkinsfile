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
				sh 'docker-compose -f docker-compose.tests.yml up -d'
			}
		}

		stage('Unit tests'){
			environment{
				ConnectionStrings__TestsDatabase = "Host=db;Port=5432;Database=devops_authservice_tests;Username=postgres;Password=postgres"
			}

			steps {
				sh 'dotnet test --filter "Category=Unit" --no-build'
			}
		}

		stage('Integration tests'){
			environment{
				ConnectionStrings__TestsDatabase = "Host=db;Port=5432;Database=devops_authservice_tests;Username=postgres;Password=postgres"
			}
			
			steps {
				sh 'dotnet test --filter "Category=Integration" --no-build'
			}
		}

		stage('Docker Compose Tests Down') {
			steps {
				sh 'docker-compose -f docker-compose.tests.yml down'
			}
		}

		stage('Docker Compose Build Dev') {
			when {
				branch 'dev'
			}

			steps {
				sh '''
					docker-compose -f docker-compose.development.yml down
					docker-compose -f docker-compose.development.yml build --no-cache
				'''
			}
		}

		stage('Docker Compose Build Release') {
			when {
				branch 'main'
			}

			steps {
				sh '''
					docker-compose -f docker-compose.release.yml down
					docker-compose -f docker-compose.release.yml build --no-cache
				'''
			}
		}

		stage('Run Dev') {
			when {
				branch 'dev'
			}

			steps {
				sh '''
                    docker-compose -f docker-compose.development.yml up -d
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
                    docker-compose -f docker-compose.release.yml up -d
					sleep 5
					curl -f http://localhost:5128/api/Examples/health || exit 1
				'''
			}
		}
	}
}