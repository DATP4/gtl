#[cfg(test)]
mod tests {
#[test]
fn declaration_test_1(){
let x = 5
;
assert_eq!(x, 5)
}
#[test]
fn declaration_test_2(){
let x = 5.0
;
assert_eq!(x, 5.0)
}
#[test]
fn declaration_test_3(){
let x = true
;
assert_eq!(x, true)
}
#[test]
fn binary_expression_test_1(){
let test = 1 + 1 * 7
;
assert_eq!(test, 8)
}
#[test]
fn binary_expression_test_2(){
let test = 1.5 + 1.6 / 10.0
;
assert_eq!(test, 1.66)
}
#[test]
fn binary_expression_test_3(){
let test = -5 + (4 + 3 * (4 % 5) / 1 + 5) - 3
;
assert_eq!(test, 13)
}
#[test]
fn binary_expression_test_4(){
let test = 5 % 3
;
assert_eq!(test, 2)
}
#[test]
fn boolean_expression_test_1(){
let test1 = true && false
;
assert_eq!(test1, false)
}
#[test]
fn boolean_expression_test_2(){
let test1 = 1 > 2
;
assert_eq!(test1, false)
}
#[test]
fn boolean_expression_test_3(){
let test1 = 1 != 0
;
assert_eq!(test1, true)
}
#[test]
fn boolean_expression_test_4(){
let test1 = 1 <= 2
;
assert_eq!(test1, true)
}
#[test]
fn boolean_expression_test_5(){
let test1 = (true && true) || ((true && true) == true)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_1(){
let test1 = !(true && false)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_2(){
let test1 = !(1 > 2)
;
assert_eq!(test1, true)
}
#[test]
fn logical_not_test_3(){
let test1 = !(1 != 0)
;
assert_eq!(test1, false)
}
#[test]
fn logical_not_test_4(){
let test1 = !(1 <= 2)
;
assert_eq!(test1, false)
}
#[test]
fn logical_not_test_5(){
let test1 = !(!(true && true) || ((true && true) == true))
;
assert_eq!(test1, false)
}
#[test]
fn unary_expression_test_1(){
let x = -5
;
assert_eq!(x, -5)
}
#[test]
fn unary_expression_test_2(){
let x = -5.0
;
assert_eq!(x, -5.0)
}
#[test]
fn unary_expression_test_3(){
let x = -5 - -5
;
assert_eq!(x, 0)
}
#[test]
fn unary_expression_test_4(){
let x = -5 + -5
;
assert_eq!(x, -10)
}
#[test]
fn unary_expression_test_5(){
let x = -5
;
let y = -x
;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_1(){
let y = if true {
let x = 4
;x
} else {
let x = 5
;x
}

;
assert_eq!(y, 4)
}
#[test]
fn if_else_test_2(){
let y = if false {
let x = 4
;x
} else {
let x = 5
;x
}

;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_3(){
let y = if true {
let x = 4
;x
} else if true {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 4)
}
#[test]
fn if_else_test_4(){
let y = if false {
let x = 4
;x
} else if true {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 5)
}
#[test]
fn if_else_test_5(){
let y = if false {
let x = 4
;x
} else if false {
let x = 5
;x
} else {
let x = 6
;x
}

;
assert_eq!(y, 6)
}
#[test]
fn function_test_1(){
fn int_function(x: &i32) -> i32 {
let y = *x + 10 * 5
;y - 5
}
let x = int_function(&(5))
;
assert_eq!(x, 50)
}
#[test]
fn function_test_2(){
let a = 10
;
fn int_function1(x: &i32) -> i32 {
let a = 10;
let y = *x + a * 5
;y - 5
}
let x = int_function1(&(5))
;
assert_eq!(x, 50)
}
#[test]
fn function_test_3(){
fn int_function(x: &i32) -> i32 {
fn int_function2(z: &i32) -> i32 {
*z + 5
}let y = int_function2(&(*x))
;y
}
let x = int_function(&(5))
;
assert_eq!(x, 10)
}
#[test]
fn function_test_4(){
let a = 10 + 5
;
let b = 5 + 13
;
let c = a * b
;
fn int_function(x: &i32) -> i32 {
let a = 10+5;
let b = 5+13;
let c = a*b;
c + *x
}
let x = int_function(&(10))
;
assert_eq!(x, 280)
}
}