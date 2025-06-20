# ProjectZ
---
> **젤다의 전설 - 시간의 오카리나(1998)** 을 참고한 게임 구현. 1스테이지 까지를 목표로 한다

---
## 프로젝트 기능 나열
> 이번 프로젝트를 진행하기에 앞서 구현하고자 하는 게임의 기능들을 나열 해본다

- 조작 키 (인풋시스템) - C버튼 생략. 다른 키들로 대체함
- 타이틀 씬, 게임오버 씬(리스타트, 그만두기 구현),
- 프로필 선택화면 (세이브 파일 - CSV 또는 JSON)
- 캐릭터 및 맵 에셋
- 오브젝트 에셋(코키리검,방패,루피,새총,새총알,풀, 나뭇가지 등)
- 애니메이션(캐릭터들,플레이어,보스,수영,아이템 상자, 습득)
- 컷씬(오프닝,데크나무,보스)
- 라이트맵(베이크,나비 포인트 라이트) 
- 나비
- 록온시스템(시점전환,오버랩스피어)
- TPS
- 인벤토리(아이템, 저장)
- 체력(UI, 체력 증가 및 감소 표현), 체력 회복 아이템(하트 - 필드아이템)
- 적 체력(옵션)
- 사운드(소스는 원작 고대로 써도 됨) -> 오디오매니저로 재생 타이밍 정해서 씀
- 그림자(발자국도)
- 미니맵(씨네머신 가상카메라)
- 아이템 습득(필드습득은 OnCollision, 상자 습득은 애니메이션 진행 후 Inventory로, TimeScale = 0)
- 필드 아이템 드랍(스크립터블 오브젝트)
- 점프(점프 버튼은 없음)
- 물(물에서 수영 가능 - 수영 상태로 바꿈) -> 물에서 나올 수 있음
- -> 물과 지면의 y좌표 차이에 따라 판정 (일정범위 아래면 바로 이동패턴으로 전환, 일정 높이 이상이면 기어올라가는 패턴)
-  물 상태
- UI(대화창,대화 선택 옵션 포함, 체력, 루피갯수, 미니맵, 버튼전환)
- 몬스터들(인터페이스IDamagable -> (전투후) IInteractable)
- 씬 전환
- 상호작용(스위치,나뭇가지,새총 등) - 나뭇가지는 불에 붙어야 하고, 불에 붙은 나뭇가지는 거미줄을 태울 수 있어야 함
- 새총으로 벽 스위치 발동
- 적 무적상태 구현
- 수치가 계속 조정이 필요한 것은 스크립터블 오브젝트로 생성
- 플레이어 및 몬스터 상태패턴으로 구현
- A버튼 상호작용 전환 (UI도 변해야함) - (상호작용 인터페이스 함수단위로 구성) 문열기, (대화창- 나중에)확인, 대화창 넘기기, 선택 등
- 벽에 무기가 부딪히면 팅겨져 나옴(적이 그 앞에잇으면 적부터)
- 원작과 다르게 적이 추적해 오는 몬스터 구현(NavMesh)
### 기능 구현 순서 정리
> 구현하고자 하는 기능들을 우선순위 별로 나열

전투 구현							
플레이어 구현
1. 플레이어 정보(M)	이름	공격력	체력	루피	이동속도	상태 확인 필드
	2. 플레이어 상태(M)	해당 시트에서 다룸					
	3. 플레이어 이동 및 조작(C)	해당 시트에서 다룸	인풋 시스템				
	4. 카메라						
							
적구현
1. 적 정보(M)	이름	체력	공격력			
	2. 적 상태(M)	아이들	공격상태1	공격상태2	피격		
	3. 적 로직(C)	플레이어 판단 범위	라이프사이클	플레이어를 쫒아오는 적 구현(NavMesh)			

게임오버 구현	1. 이어하기	1.1.저장하기	CSV 또는 JSON	파일은 3개			
	2.타이틀로	2.1. 저장하기	CSV 또는 JSON				

필드 아이템 구현	4. 필드 아이템 OnCollision 구현	스크립터블 오브젝트	데이터는 CSV에서 불러옴				

테스트 씬 구성	필요한 모든 기능들을 테스트 할 수 있는 테스트 씬 구성	전투	상호작용	상태변경	물, 공중, 점프 등등		

레벨 구현	1. 에셋 배치						
	2. 던전						
	3. 마을						
	4. 씬전환 기능						

### 도전과제
> 이 아래는 시간이 남는다면 구현

사운드	필요한 곳들에 효과음 이벤트						
	0. 인벤토리						

상호작용 구현	1. 나비 및 주목 시스템	나비 평상시 상태(플레이어 주위 Patrol = 돌리트랙)	주목 시 카메라가 버추얼 카메라 주시 카메라로 전환(플레이어와 대상을 한 샷에 담음)	플레이어의 공격과 이동이 해당 대상을 기준으로 이루어짐	새총도 대상에게 날아감		
	1.1 나비는 주목 Position으로 날라감. 아닌 경우 되돌아옴	주목 시 상대에게 MoveForwards 이후 Patrol					
	2. 물체 및 NPC와 상호작용	보스 전의 적은 처치 후 NPC 생성					
	3. 여러 상호작용 상태들 구현	나뭇가지 끝에 Collider 추가. 일반 나뭇가지 상태(무기), 불에OnCollision -> 불붙은 나뭇가지	벽 스위치는 Idamagable	NPC와 대화 상태	조사 상태	물건을 집을 수 있게 상호작용 상태	

라이트	2. 라이트 배치						
	3. 라이트 맵 생성						

레벨 구현	3. 마을의 방들						

씨네머신 구현	각 컷씬 구현						

미니맵	버추얼 카메라로 캐릭터 아이콘과 지도 이미지로 구현


---
## 플레이어 구현
- 레퍼런스 게임에서 플레이어가 조작이 가능한 부분들을 기능 나열
- 
### 플레이어 상태 구현
- **HFSM** 을 사용해 상태 전이를 구현
- 자세한 내용은 Project_Design 폴더의 Features 엑셀 파일의 **조작 상태** 참조
### 플레이어 조작 구현
- 인풋 시스템을 활용 ( 모든 조작은 인풋 시스템으로 키 바인딩)
- 인풋 시스템의 인풋 액션을 활용 ( 플레이어의 공격, 플레이어의 차지 공격 등)

## 그외
- 이외의 자세한 내용은 Features 엑셀 파일 참조
