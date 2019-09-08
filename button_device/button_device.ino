#define LED_RED 5
#define LED_GREEN0 2
#define LED_GREEN1 3
#define LED_GREEN2 4
#define LARGE_SWITCH 6
#define MODE_SWITCH 7

#define BAUDRAIT 19200

#define MAIN_MSG "I got arrived"
#define TEST_MSG "test         "

#define MAIN_MODE 0
#define PRESSED 0
#define RELEASED 1

int large_switch_state = RELEASED;
char header[3];
char body[4];
int header_pos = 0;
int body_pos = 0;

void setup() {
    Serial.begin(BAUDRAIT);
    pinMode(LED_RED, OUTPUT);
    pinMode(LED_GREEN0, OUTPUT);
    pinMode(LED_GREEN1, OUTPUT);
    pinMode(LED_GREEN2, OUTPUT);
    pinMode(LARGE_SWITCH, INPUT);
    pinMode(MODE_SWITCH, INPUT);
}

void loop() {
    recive();

    // send message if large switch is pressed
    int mode = digitalRead(MODE_SWITCH);
    int current_large_switch = digitalRead(LARGE_SWITCH);
    if(mode == MAIN_MODE) {
        if(large_switch_state == RELEASED && current_large_switch == PRESSED) {
            Serial.print(MAIN_MSG);
        }
    }
    else {
        if(large_switch_state == RELEASED && current_large_switch == PRESSED) {
            Serial.print(TEST_MSG);
        }
    }
    large_switch_state = current_large_switch;
}

void recive() {
    // check header
    for(int i = header_pos; i < 3; i++) {
        if(Serial.available() <= 0) { return; }
        header[i] = Serial.read();
        header_pos++;
    }

    int check_header = header[0] == 'M' && header[1] == 'S' && header[2] == 'G';
    if(!check_header) { return; }
    
    // check body
    for(int i = body_pos; i < 4; i++) {
        if(Serial.available() <= 0) { break; }
        body[i] = Serial.read();
        body_pos++;
    }
    int check_ping = body[0] == 'P' && body[1] == 'i' && body[2] == 'n' && body[3] == 'g';
    if(check_ping) {
        clear_header_buf();
        clear_body_buf();
        Serial.write("MSGPing");
        return;
    }
    int check_connected = body[0] == 'C' && body[1] == 'N' && body[2] == 'C' && body[3] == 'T';
    if(check_connected) {
        clear_header_buf();
        clear_body_buf();
        digitalWrite(LED_RED, HIGH);
        return;
    }
    // DSCN
    int check_disconnected = body[0] == 'D' && body[1] == 'S' && body[2] == 'C' && body[3] == 'N';
    if(check_disconnected) {
        clear_header_buf();
        clear_body_buf();
        digitalWrite(LED_RED, LOW);
        return;
    }
    int check_ok = body[0] == 'O' && body[1] == 'K';
    if(check_ok) {
        clear_header_buf();
        clear_body_buf();
        success_led();
        return;
    }
}

void clear_header_buf(){
    header_pos = 0;
    for(int i = 0; i < 3; i++) {
        header[i] = 0;
    }
}

void clear_body_buf(){
    body_pos = 0;
    for(int i = 0; i < 4; i++) {
        body[i] = 0;
    }
}

void success_led() {
    for(int i = 0; i < 8; i++) {
        digitalWrite(LED_GREEN0, HIGH);
        digitalWrite(LED_GREEN1, HIGH);
        digitalWrite(LED_GREEN2, HIGH);
        delay(300);
        digitalWrite(LED_GREEN0, LOW);
        digitalWrite(LED_GREEN1, LOW);
        digitalWrite(LED_GREEN2, LOW);
        delay(300);
    }
}
